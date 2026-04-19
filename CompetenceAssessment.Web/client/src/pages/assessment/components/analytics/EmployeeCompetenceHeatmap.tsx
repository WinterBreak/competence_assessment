import React, { useState, useMemo, useEffect } from 'react';
import { Tooltip, Select, Button } from '@carbon/react';
import { EmployeeData, EmployeeCompetenceData } from "../../types/assessment.types";
import { assessmentService } from "../../services/assessmentService";

interface CompetenceMatrixProps {
    onEmployeeClick: (employeeId: string) => void;
}

export const EmployeeCompetenceHeatmap: React.FC<CompetenceMatrixProps> = ({
                                                                               onEmployeeClick
                                                                           }) => {
    const [selectedDepartment, setSelectedDepartment] = useState<string>('all');
    const [sortBy, setSortBy] = useState<'fullName' | 'avgScore'>('fullName');
    const [employees, setEmployees] = useState<EmployeeData[]>([]);

    // Получаем список всех компетенций из данных
    const competencies = useMemo(() => {
        const allComps = new Set<string>();
        employees.forEach(emp => {
            emp.competencies.forEach(comp => {
                allComps.add(comp.name);
            });
        });
        return Array.from(allComps);
    }, [employees]);

    useEffect(() => {
        assessmentService.getParticipantCompetencies().then(data => {
            setEmployees(data);
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

    const filteredEmployees = selectedDepartment === 'all'
        ? employees
        : employees.filter(e => e.department === selectedDepartment);

    const departments = useMemo(() =>
            ['all', ...Array.from(new Set(employees.map(e => e.department)))],
        [employees]
    );

    // Функция для получения компетенции сотрудника по имени
    const getEmployeeCompetence = (employee: EmployeeData, competenceName: string): EmployeeCompetenceData | undefined => {
        return employee.competencies.find(c => c.name === competenceName);
    };

    // Функция для расчета среднего балла
    const getAverageScore = (employee: EmployeeData) => {
        if (employee.competencies.length === 0) return 0;
        const sum = employee.competencies.reduce((total, comp) => total + comp.percentage, 0);
        return sum / employee.competencies.length;
    };

    return (
        <div style={{ padding: '2rem', overflowX: 'auto' }}>
            <div style={{ marginBottom: '2rem', display: 'flex', gap: '1rem', alignItems: 'center' }}>
                <Select
                    id="department-filter"
                    labelText="Департамент"
                    value={selectedDepartment}
                    onChange={(e) => setSelectedDepartment(e.target.value)}
                    style={{ width: '200px' }}
                >
                    {departments.map(dept => (
                        <option key={dept} value={dept}>
                            {dept === 'all' ? 'Все департаменты' : dept}
                        </option>
                    ))}
                </Select>

                <Button
                    kind="secondary"
                    onClick={() => setSortBy(sortBy === 'fullName' ? 'avgScore' : 'fullName')}
                >
                    Сортировать по {sortBy === 'fullName' ? 'среднему баллу' : 'имени'}
                </Button>
            </div>

            <table style={{
                borderCollapse: 'collapse',
                width: '100%',
                boxShadow: '0 1px 3px rgba(0,0,0,0.1)'
            }}>
                <thead>
                <tr style={{ backgroundColor: '#f4f4f4' }}>
                    <th style={{
                        padding: '1rem',
                        position: 'sticky',
                        left: 0,
                        backgroundColor: '#f4f4f4',
                        zIndex: 1
                    }}>
                        Сотрудник / Компетенция
                    </th>
                    {competencies.map(comp => (
                        <th key={comp} style={{
                            padding: '1rem',
                            minWidth: '120px'
                        }}>
                            {comp}
                        </th>
                    ))}
                    <th style={{ padding: '1rem', backgroundColor: '#f4f4f4' }}>
                        Средний балл
                    </th>
                </tr>
                </thead>
                <tbody>
                {filteredEmployees
                    .sort((a, b) => {
                        if (sortBy === 'fullName') {
                            return a.fullName.localeCompare(b.fullName);
                        } else {
                            const avgA = getAverageScore(a);
                            const avgB = getAverageScore(b);
                            return avgB - avgA;
                        }
                    })
                    .map(employee => {
                        const avgScore = getAverageScore(employee);

                        return (
                            <tr
                                key={employee.userId}
                                onClick={() => onEmployeeClick(employee.userId)}
                                style={{
                                    cursor: 'pointer',
                                    transition: 'background-color 0.2s',
                                    borderBottom: '1px solid #e0e0e0'
                                }}
                                onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#f4f4f4'}
                                onMouseLeave={(e) => e.currentTarget.style.backgroundColor = 'white'}
                            >
                                <td style={{
                                    padding: '0.75rem',
                                    position: 'sticky',
                                    left: 0,
                                    backgroundColor: 'inherit',
                                    fontWeight: 'bold'
                                }}>
                                    {employee.fullName}
                                    <div style={{ fontSize: '0.75rem', color: '#6f6f6f' }}>
                                        {employee.position}
                                    </div>
                                </td>

                                {competencies.map(compName => {
                                    const competence = getEmployeeCompetence(employee, compName);
                                    if (!competence) return <td key={compName} style={{ padding: '0.75rem', textAlign: 'center' }}>—</td>;

                                    return (
                                        <td key={compName} style={{ padding: '0.75rem', textAlign: 'center' }}>
                                            <Tooltip label={getLevelText(competence.percentage)}>
                                                <div style={{
                                                    width: '40px',
                                                    height: '40px',
                                                    borderRadius: '8px',
                                                    backgroundColor: getColorByLevel(competence.percentage),
                                                    margin: '0 auto',
                                                    display: 'flex',
                                                    alignItems: 'center',
                                                    justifyContent: 'center',
                                                    color: 'white',
                                                    fontWeight: 'bold',
                                                    transition: 'transform 0.2s',
                                                    cursor: 'pointer'
                                                }}
                                                     onMouseEnter={(e) => e.currentTarget.style.transform = 'scale(1.1)'}
                                                     onMouseLeave={(e) => e.currentTarget.style.transform = 'scale(1)'}
                                                >
                                                    {Math.round(competence.percentage)}%
                                                </div>
                                            </Tooltip>
                                        </td>
                                    );
                                })}

                                <td style={{
                                    padding: '0.75rem',
                                    textAlign: 'center',
                                    fontWeight: 'bold',
                                    backgroundColor: '#f9f9f9'
                                }}>
                                    <div style={{
                                        width: '100%',
                                        height: '4px',
                                        backgroundColor: '#e0e0e0',
                                        borderRadius: '2px',
                                        marginBottom: '0.5rem'
                                    }}>
                                        <div style={{
                                            width: `${avgScore}%`,
                                            height: '100%',
                                            backgroundColor: getColorByLevel(avgScore),
                                            borderRadius: '2px'
                                        }} />
                                    </div>
                                    {Math.round(avgScore)}%
                                </td>
                            </tr>
                        );
                    })}
                </tbody>
            </table>
        </div>
    );
};