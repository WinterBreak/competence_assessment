// components/Admin/CompetenceMatrix/PositionCompetenceMatrix.tsx
import React, { useState } from 'react';
import {
    BarChart,
    Bar,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    Legend,
    ResponsiveContainer,
    Cell
} from 'recharts';
import { Accordion, AccordionItem, Tag, Select } from '@carbon/react';

interface PositionRequirement {
    position: string;
    requiredCompetencies: Map<string, {
        requiredLevel: number;
        minPercentage: number;
    }>;
}

interface EmployeePositionData {
    employeeId: string;
    name: string;
    position: string;
    competencies: Map<string, number>;
}

interface PositionCompetenceMatrixProps {
    positions: PositionRequirement[];
    employees: EmployeePositionData[];
}

export const PositionCompetenceMatrix: React.FC<PositionCompetenceMatrixProps> = ({
                                                                                      positions,
                                                                                      employees
                                                                                  }) => {
    const [selectedPosition, setSelectedPosition] = useState<string>(positions[0]?.position || '');

    const getGapStatus = (current: number, required: number) => {
        const gap = current - required;
        if (gap >= 0) return { status: 'success', text: 'Соответствует', icon: '✓' };
        if (gap >= -10) return { status: 'warning', text: 'Незначительное отставание', icon: '⚠' };
        return { status: 'error', text: 'Требуется развитие', icon: '✗' };
    };

    const positionData = positions.find(p => p.position === selectedPosition);
    const employeesInPosition = employees.filter(e => e.position === selectedPosition);

    // Данные для графика Gap Analysis
    const gapAnalysisData = Array.from(positionData?.requiredCompetencies.entries() || [])
        .map(([competence, requirement]) => {
            const avgScore = employeesInPosition.reduce((sum, emp) =>
                sum + (emp.competencies.get(competence) || 0), 0) / employeesInPosition.length;

            return {
                competence,
                required: requirement.minPercentage,
                current: avgScore,
                gap: avgScore - requirement.minPercentage
            };
        });

    return (
        <div style={{ padding: '2rem' }}>
            <h2>Матрица компетенций по должностям</h2>

            <div style={{ marginBottom: '2rem' }}>
                <Select
                    id="position-filter"
                    labelText="Выберите должность"
                    value={selectedPosition}
                    onChange={(e) => setSelectedPosition(e.target.value)}
                >
                    {positions.map(pos => (
                        <option key={pos.position} value={pos.position}>
                            {pos.position} ({employees.filter(e => e.position === pos.position).length} чел.)
                        </option>
                    ))}
                </Select>
            </div>

            {/* Gap Analysis Chart */}
            <div style={{ marginBottom: '2rem', height: '400px' }}>
                <h3>Анализ разрыва компетенций</h3>
                <ResponsiveContainer>
                    <BarChart data={gapAnalysisData}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="competence" angle={-45} textAnchor="end" height={100} />
                        <YAxis label={{ value: 'Процент выполнения (%)', angle: -90, position: 'insideLeft' }} />
                        <Tooltip />
                        <Legend />
                        <Bar dataKey="required" name="Требуемый уровень" fill="#8d8d8d" />
                        <Bar dataKey="current" name="Текущий уровень" fill="#0f62ac" />
                    </BarChart>
                </ResponsiveContainer>
            </div>

            {/* Employee vs Requirements Matrix */}
            <div style={{ overflowX: 'auto' }}>
                <h3>Соответствие сотрудников требованиям должности</h3>
                <table style={{ width: '100%', borderCollapse: 'collapse', marginTop: '1rem' }}>
                    <thead>
                    <tr style={{ backgroundColor: '#f4f4f4' }}>
                        <th style={{ padding: '1rem' }}>Сотрудник</th>
                        {Array.from(positionData?.requiredCompetencies.keys() || []).map(comp => (
                            <th key={comp} style={{ padding: '1rem' }}>{comp}</th>
                        ))}
                        <th style={{ padding: '1rem' }}>Общий статус</th>
                    </tr>
                    </thead>
                    <tbody>
                    {employeesInPosition.map(employee => {
                        const overallStatus = Array.from(positionData?.requiredCompetencies.entries() || [])
                            .every(([comp, req]) =>
                                (employee.competencies.get(comp) || 0) >= req.minPercentage
                            );

                        return (
                            <tr key={employee.employeeId} style={{ borderBottom: '1px solid #e0e0e0' }}>
                                <td style={{ padding: '1rem', fontWeight: 'bold' }}>{employee.name}</td>
                                {Array.from(positionData?.requiredCompetencies.entries() || []).map(([comp, req]) => {
                                    const currentScore = employee.competencies.get(comp) || 0;
                                    const meetsRequirement = currentScore >= req.minPercentage;

                                    return (
                                        <td key={comp} style={{ padding: '1rem', textAlign: 'center' }}>
                                            <Tag type={meetsRequirement ? 'green' : 'red'}>
                                                {currentScore}% / {req.minPercentage}%
                                            </Tag>
                                        </td>
                                    );
                                })}
                                <td style={{ padding: '1rem', textAlign: 'center' }}>
                                    <Tag type={overallStatus ? 'green' : 'magenta'}>
                                        {overallStatus ? 'Соответствует' : 'Требует внимания'}
                                    </Tag>
                                </td>
                            </tr>
                        );
                    })}
                    </tbody>
                </table>
            </div>
        </div>
    );
};