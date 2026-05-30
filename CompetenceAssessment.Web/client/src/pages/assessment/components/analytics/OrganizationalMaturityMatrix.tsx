import React, { useState, useEffect } from 'react';
import { ResponsiveRadar } from '@nivo/radar';
import {Select, Loading, Button} from "@carbon/react";
import {assessmentService} from "../../services/assessmentService";

interface CompetenceData {
    competenceId: string;
    competenceName: string;
    score: number;
    percentage: number;
    level: string;
}

interface DepartmentData {
    departmentId: string;
    departmentName: string;
    score: number;
    percentage: number;
    level: string;
    competencies: CompetenceData[];
    employees: any[];
}

const MATURITY_LEVELS = {
    initial: { name: 'Начальный', score: 20, color: '#da1e28', description: 'Процессы непредсказуемы и слабо контролируемы' },
    managed: { name: 'Управляемый', score: 40, color: '#f1c21b', description: 'Процессы планируются и отслеживаются' },
    defined: { name: 'Определенный', score: 60, color: '#ff832b', description: 'Процессы стандартизированы' },
    quantitatively: { name: 'Количественно управляемый', score: 80, color: '#0f62ac', description: 'Процессы измеряются и контролируются' },
    optimizing: { name: 'Оптимизирующий', score: 100, color: '#198038', description: 'Процессы постоянно улучшаются' }
};

const getMaturityLevelByScore = (percentage: number): keyof typeof MATURITY_LEVELS => {
    if (percentage >= 80) return 'optimizing';
    if (percentage >= 60) return 'quantitatively';
    if (percentage >= 40) return 'defined';
    if (percentage >= 20) return 'managed';
    return 'initial';
};

export const OrganizationalMaturityMatrix: React.FC = () => {
    const [departments, setDepartments] = useState<DepartmentData[]>([]);
    const [loading, setLoading] = useState(true);
    const [selectedDepartmentId, setSelectedDepartmentId] = useState<string>('');

    useEffect(() => {
        const fetchDepartmentsData = async () => {
            try {
                setLoading(true);
                const response = await assessmentService.getDepartmentsCompetencies();
                setDepartments(response);
                if (response.length > 0 && !selectedDepartmentId) {
                    setSelectedDepartmentId(response[0].departmentId);
                }
            } catch (error) {
                console.error('Ошибка загрузки данных департаментов:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchDepartmentsData();
    }, []);

    if (loading) {
        return (
            <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '400px' }}>
                <Loading description="Загрузка матрицы зрелости..." />
            </div>
        );
    }

    if (!departments || departments.length == 0) {
        return (
            <div style={{ padding: '2rem', textAlign: 'center', backgroundColor: 'white', borderRadius: '8px' }}>
                <h3>Нет данных о департаментах</h3>
                <p>Для отображения матрицы зрелости необходимо загрузить данные по департаментам.</p>
            </div>
        );
    }

    const selectedDepartment = departments.find(d => d.departmentId == selectedDepartmentId);
    if (!selectedDepartment) return null;

    const getMaturityData = () => {
        return selectedDepartment.competencies.map(comp => ({
            competence: comp.competenceName,
            уровень: comp.percentage,
            уровеньНазвание: MATURITY_LEVELS[getMaturityLevelByScore(comp.percentage)].name,
            уровеньУровень: getMaturityLevelByScore(comp.percentage)
        }));
    };

    const data = getMaturityData();
    const overallMaturity = getMaturityLevelByScore(selectedDepartment.percentage);

    const radarData = data.map(item => ({
        competence: item.competence,
        'Текущий уровень': item.уровень
    }));

    return (
        <div style={{padding: '2rem', overflowX: 'auto' }}>
            <div style={{ marginBottom: '2rem', display: 'flex', gap: '1rem', alignItems: 'center' }}>
                <Select
                    id="department-filter"
                    labelText="Выберите департамент"
                    style={{ width: '400px' }}
                    value={selectedDepartmentId}
                    onChange={(e) => setSelectedDepartmentId(e.target.value)}
                >
                    {departments.map(dept => (
                        <option key={dept.departmentId} value={dept.departmentId}>
                            {dept.departmentName} (Уровень: {MATURITY_LEVELS[getMaturityLevelByScore(dept.percentage)].name})
                        </option>
                    ))}
                </Select>

                <Button
                    kind="primary"
                    onClick={async () => {
                        await assessmentService.exportAssessments({
                            reportType: '2',
                            departmentId: selectedDepartmentId
                        });
                    }}
                >
                    Экспорт
                </Button>
            </div>

            <div style={{
                display: 'grid',
                gridTemplateColumns: '1fr 1fr',
                gap: '2rem',
                marginBottom: '2rem'
            }}>
                <div style={{ height: '500px' }}>
                    <h3>Профиль зрелости компетенций</h3>
                    <ResponsiveRadar
                        data={radarData}
                        keys={['Текущий уровень']}
                        indexBy="competence"
                        valueFormat=">-.0f"
                        margin={{ top: 70, right: 80, bottom: 40, left: 80 }}
                        borderColor={{ from: 'color' }}
                        gridLabelOffset={36}
                        dotSize={10}
                        dotColor={{ theme: 'background' }}
                        dotBorderWidth={2}
                        colors={{ scheme: 'nivo' }}
                        fillOpacity={0.25}
                        blendMode="multiply"
                        animate={true}
                        motionConfig="wobbly"
                        isInteractive={true}
                        theme={{
                            tooltip: {
                                container: {
                                    background: 'white',
                                    color: '#333',
                                    fontSize: '12px',
                                    borderRadius: '4px',
                                    boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
                                    padding: '8px 12px'
                                }
                            }
                        }}
                        legends={[
                            {
                                anchor: 'top-left',
                                direction: 'column',
                                translateX: -50,
                                translateY: -40,
                                itemWidth: 80,
                                itemHeight: 20,
                                symbolSize: 12,
                                symbolShape: 'circle'
                            }
                        ]}
                    />
                </div>

                <div style={{
                    padding: '1.5rem',
                    backgroundColor: '#f4f4f4',
                    borderRadius: '8px'
                }}>
                    <h3>Общий уровень зрелости</h3>
                    <div style={{
                        textAlign: 'center',
                        marginTop: '1rem'
                    }}>
                        <div style={{
                            width: '150px',
                            height: '150px',
                            borderRadius: '50%',
                            backgroundColor: MATURITY_LEVELS[overallMaturity].color,
                            margin: '0 auto',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            color: 'white',
                            fontSize: '1.5rem',
                            fontWeight: 'bold',
                            textAlign: 'center',
                            padding: '1rem'
                        }}>
                            {MATURITY_LEVELS[overallMaturity].name.split(' ').map(word => word.slice(0, 8))[0]}
                        </div>
                        <p style={{ marginTop: '1rem', color: '#6f6f6f' }}>
                            {MATURITY_LEVELS[overallMaturity].description}
                        </p>
                        <div style={{ marginTop: '1rem' }}>
                            <strong>{Math.round(selectedDepartment.percentage)}%</strong>
                            <div style={{
                                width: '100%',
                                height: '8px',
                                backgroundColor: '#e0e0e0',
                                borderRadius: '4px',
                                marginTop: '0.5rem',
                                overflow: 'hidden'
                            }}>
                                <div style={{
                                    width: `${selectedDepartment.percentage}%`,
                                    height: '100%',
                                    backgroundColor: MATURITY_LEVELS[overallMaturity].color,
                                    transition: 'width 0.5s ease'
                                }} />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div>
                <h3>Детализация по компетенциям</h3>
                {data.map(item => {
                    const level = getMaturityLevelByScore(item.уровень);
                    return (
                        <div key={item.competence} style={{ marginBottom: '1rem' }}>
                            <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '0.5rem' }}>
                                <span>{item.competence}</span>
                                <span style={{ color: MATURITY_LEVELS[level].color }}>
                                    {item.уровеньНазвание} ({item.уровень.toFixed(2)}%)
                                </span>
                            </div>
                            <div style={{
                                width: '100%',
                                height: '30px',
                                backgroundColor: '#e0e0e0',
                                borderRadius: '4px',
                                overflow: 'hidden'
                            }}>
                                <div style={{
                                    width: `${item.уровень}%`,
                                    height: '100%',
                                    backgroundColor: MATURITY_LEVELS[level].color,
                                    transition: 'width 0.5s ease'
                                }} />
                            </div>
                        </div>
                    );
                })}
            </div>

            <div style={{
                marginTop: '2rem',
                padding: '1rem',
                backgroundColor: '#e8f4ff',
                borderRadius: '8px'
            }}>
                <h4>📊 Рекомендации по повышению зрелости</h4>
                <ul style={{ marginTop: '1rem', lineHeight: '1.8' }}>
                    {data.filter(item => item.уровень < 60).map(item => (
                        <li key={item.competence}>
                            <strong>{item.competence}</strong>: {item.уровеньНазвание} уровень.
                            Рекомендуется разработать программу развития и внедрить KPI.
                        </li>
                    ))}
                    {data.filter(item => item.уровень < 60).length == 0 && (
                        <li>Отличный результат! Все компетенции на высоком уровне зрелости.</li>
                    )}
                </ul>
            </div>
        </div>
    );
};