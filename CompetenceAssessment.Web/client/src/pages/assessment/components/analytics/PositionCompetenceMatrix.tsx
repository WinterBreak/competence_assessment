import React, { useEffect, useState } from 'react';
import {
    BarChart,
    Bar,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    Legend,
    ResponsiveContainer
} from 'recharts';
import { Accordion, AccordionItem, Tag, Select } from '@carbon/react';
import { assessmentService } from "../../services/assessmentService";
import { EmployeeData, PositionData, EmployeeCompetenceData } from "../../types/assessment.types";

export const PositionCompetenceMatrix: React.FC = () => {
    const [positions, setPositions] = useState<PositionData[]>([]);
    const [participants, setParticipants] = useState<EmployeeData[]>([]);
    const [selectedPosition, setSelectedPosition] = useState<string>('');

    useEffect(() => {
        assessmentService.getParticipantCompetencies().then(data => {
            setParticipants(data);
        });
        assessmentService.getPositionCompetencies().then(data => {
            setPositions(data);
            if (data && data.length > 0) {
                setSelectedPosition(data[0]?.positionName || '');
            }
        });
    }, []);

    const getColorByLevel = (percentage: number) => {
        if (percentage >= 80) return '#0f62ac';
        if (percentage >= 60) return '#ff832b';
        if (percentage >= 40) return '#f1c21b';
        return '#da1e28';
    };

    const getLevelText = (percentage: number) => {
        if (percentage >= 80) return 'Эксперт';
        if (percentage >= 60) return 'Продвинутый';
        if (percentage >= 40) return 'Средний';
        return 'Начальный';
    };

    const positionData = positions.find(p => p.positionName === selectedPosition);

    // Данные для графика Gap Analysis
    const gapAnalysisData = (positionData?.competencies || [])
        .map(competence => {
            return {
                competence: competence.competenceName,
                required: 40,
                current: competence.score,
                gap: competence.percentage
            };
        });

    // Функция для получения текущей компетенции сотрудника
    const getCurrentScore = (employeeId: string, competenceId: string): number => {
        const employee = participants.find(p => p.userId === employeeId);
        const competence = employee?.competencies.find(c => c.competenceId === competenceId);
        return competence?.percentage || 0;
    };

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
                        <option key={pos.positionName} value={pos.positionName}>
                            {pos.positionName} ({pos.employees?.length || 0} чел.)
                        </option>
                    ))}
                </Select>
            </div>

            {/* Gap Analysis Chart */}
            {gapAnalysisData.length > 0 && (
                <div style={{ marginBottom: '2rem', height: '400px' }}>
                    <h3>Анализ разрыва компетенций</h3>
                    <ResponsiveContainer width="100%" height="100%">
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
            )}

            <div style={{ overflowX: 'auto' }}>
                <h3>Соответствие сотрудников требованиям должности</h3>
                <table style={{ width: '100%', borderCollapse: 'collapse', marginTop: '1rem' }}>
                    <thead>
                    <tr style={{ backgroundColor: '#f4f4f4' }}>
                        <th style={{ padding: '1rem' }}>Сотрудник</th>
                        {(positionData?.competencies || []).map(comp => (
                            <th key={comp?.competenceId} style={{ padding: '1rem' }}>{comp.competenceName}</th>
                        ))}
                        <th style={{ padding: '1rem' }}>Общий статус</th>
                    </tr>
                    </thead>
                    <tbody>
                    {(positionData?.employees || []).map(employee => {
                        const overallStatus = (positionData?.competencies || []).every(comp => {
                            const currentScore = getCurrentScore(employee.id, comp.competenceId);
                            return currentScore >= 39;
                        });

                        return (
                            <tr key={employee.id} style={{ borderBottom: '1px solid #e0e0e0' }}>
                                <td style={{ padding: '1rem', fontWeight: 'bold' }}>{employee.fullName}</td>
                                {(positionData?.competencies || []).map(comp => {
                                    const currentScore = getCurrentScore(employee.id, comp.competenceId);
                                    const meetsRequirement = currentScore >= 39;

                                    return (
                                        <td key={comp.competenceId} style={{ padding: '1rem', textAlign: 'center' }}>
                                            <Tag type={meetsRequirement ? 'green' : 'red'}>
                                                {`${Math.round(currentScore)}% / 39%`}
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