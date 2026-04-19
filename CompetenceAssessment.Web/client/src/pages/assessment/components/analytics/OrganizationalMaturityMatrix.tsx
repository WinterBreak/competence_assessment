import React from 'react';
import { ResponsiveRadar } from '@nivo/radar';
import { Select } from "@carbon/react";

interface DepartmentMaturity {
    department: string;
    competencies: Map<string, {
        score: number;
        maturityLevel: 'initial' | 'managed' | 'defined' | 'quantitatively' | 'optimizing';
    }>;
}

interface MaturityMatrixProps {
    departments: DepartmentMaturity[];
    benchmark?: Map<string, number>;
}

const MATURITY_LEVELS = {
    initial: { name: 'Начальный', score: 20, color: '#da1e28', description: 'Процессы непредсказуемы и слабо контролируемы' },
    managed: { name: 'Управляемый', score: 40, color: '#f1c21b', description: 'Процессы планируются и отслеживаются' },
    defined: { name: 'Определенный', score: 60, color: '#ff832b', description: 'Процессы стандартизированы' },
    quantitatively: { name: 'Количественно управляемый', score: 80, color: '#0f62ac', description: 'Процессы измеряются и контролируются' },
    optimizing: { name: 'Оптимизирующий', score: 100, color: '#198038', description: 'Процессы постоянно улучшаются' }
};

export const OrganizationalMaturityMatrix: React.FC<MaturityMatrixProps> = ({ departments, benchmark }) => {
    const [selectedDepartment, setSelectedDepartment] = React.useState<string>(departments[0]?.department || '');

    const getMaturityData = (department: string) => {
        const dept = departments.find(d => d.department === department);
        if (!dept) return [];

        return Array.from(dept.competencies.entries()).map(([competence, data]) => ({
            competence: competence,
            уровень: MATURITY_LEVELS[data.maturityLevel].score,
            уровеньНазвание: MATURITY_LEVELS[data.maturityLevel].name,
            бенчмарк: benchmark?.get(competence) || 0
        }));
    };

    const getOverallMaturity = (department: string) => {
        const dept = departments.find(d => d.department === department);
        if (!dept) return 'initial';

        const scores = Array.from(dept.competencies.values()).map(v => MATURITY_LEVELS[v.maturityLevel].score);
        const avgScore = scores.reduce((a, b) => a + b, 0) / scores.length;

        let maturity: keyof typeof MATURITY_LEVELS = 'initial';
        for (const [level, data] of Object.entries(MATURITY_LEVELS)) {
            if (avgScore >= data.score) {
                maturity = level as keyof typeof MATURITY_LEVELS;
            }
        }
        return maturity;
    };

    const data = getMaturityData(selectedDepartment);
    const overallMaturity = getOverallMaturity(selectedDepartment);

    // Подготовка данных для Nivo Radar
    const radarData = data.map(item => ({
        competence: item.competence,
        'Текущий уровень': item.уровень,
        ...(benchmark && { 'Отраслевой бенчмарк': item.бенчмарк })
    }));

    const radarKeys = benchmark
        ? ['Текущий уровень', 'Отраслевой бенчмарк']
        : ['Текущий уровень'];

    return (
        <div style={{ padding: '2rem' }}>
            <h2>Матрица зрелости компетенций организации</h2>

            <div style={{ marginBottom: '2rem' }}>
                <Select
                    id="department-filter"
                    labelText="Выберите департамент"
                    value={selectedDepartment}
                    onChange={(e) => setSelectedDepartment(e.target.value)}
                >
                    {departments.map(dept => (
                        <option key={dept.department} value={dept.department}>
                            {dept.department} (Уровень: {MATURITY_LEVELS[getOverallMaturity(dept.department)].name})
                        </option>
                    ))}
                </Select>
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
                        keys={radarKeys}
                        indexBy="competence"
                        valueFormat=">-.0f"
                        margin={{ top: 70, right: 80, bottom: 40, left: 80 }}
                        borderColor={{ from: 'color' }}
                        gridLabelOffset={36}
                        dotSize={10}
                        dotColor={{ theme: 'background' }}
                        dotBorderWidth={2}
                        colors={benchmark ? ['#0f62ac', '#da1e28'] : { scheme: 'nivo' }}
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
                            <strong>{Math.round(MATURITY_LEVELS[overallMaturity].score)}%</strong>
                            <div style={{
                                width: '100%',
                                height: '8px',
                                backgroundColor: '#e0e0e0',
                                borderRadius: '4px',
                                marginTop: '0.5rem',
                                overflow: 'hidden'
                            }}>
                                <div style={{
                                    width: `${MATURITY_LEVELS[overallMaturity].score}%`,
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
                {data.map(item => (
                    <div key={item.competence} style={{ marginBottom: '1rem' }}>
                        <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '0.5rem' }}>
                            <span>{item.competence}</span>
                            <span style={{ color: MATURITY_LEVELS[Object.keys(MATURITY_LEVELS).find(
                                    k => MATURITY_LEVELS[k as keyof typeof MATURITY_LEVELS].score === item.уровень
                                ) as keyof typeof MATURITY_LEVELS]?.color }}>
                                {item.уровеньНазвание} ({item.уровень}%)
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
                                backgroundColor: MATURITY_LEVELS[Object.keys(MATURITY_LEVELS).find(
                                    k => MATURITY_LEVELS[k as keyof typeof MATURITY_LEVELS].score === item.уровень
                                ) as keyof typeof MATURITY_LEVELS]?.color,
                                transition: 'width 0.5s ease'
                            }} />
                        </div>
                        {benchmark && benchmark.get(item.competence) && (
                            <div style={{
                                marginTop: '0.25rem',
                                fontSize: '0.75rem',
                                color: '#6f6f6f'
                            }}>
                                Бенчмарк: {benchmark.get(item.competence)}%
                            </div>
                        )}
                    </div>
                ))}
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
                    {data.filter(item => item.уровень < 60).length === 0 && (
                        <li>Отличный результат! Все компетенции на высоком уровне зрелости.</li>
                    )}
                </ul>
            </div>
        </div>
    );
};