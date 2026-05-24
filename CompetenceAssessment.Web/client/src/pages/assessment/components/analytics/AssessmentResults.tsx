// components/Results/AssessmentResults.tsx
import React, { useState } from 'react';
import { TotalScoreGauge } from './TotalScoreGauge';
import { CompetenciesRadar } from './CompetenciesRadar';
import { CompetencyBar } from './CompetencyBar';
import { TestingResultsView} from './CompetenciesTable';
import {Assessment, AssessmentCalculation} from "../../types/assessment.types";

interface AssessmentResultsProps {
    calculation: AssessmentCalculation;
    assessment: Assessment;
}

export const AssessmentResults: React.FC<AssessmentResultsProps> = ({
                                                                        calculation, assessment
                                                                    }) => {
    const [viewType, setViewType] = useState<'chart' | 'list'>('chart');
    const [activeTab, setActiveTab] = useState<'radar' | 'detail'>('radar');

    const competencies = new Map();
    Object.keys(calculation.competenciesReceivedPercentage).forEach(key => {
        const id = parseInt(key);
        competencies.set(calculation.competenceNames[id] || `Компетенция ${id}`, {
            reference: calculation.competenceReferences[id],
            received: calculation.competenciesReceived[id],
            percentage: calculation.competenciesReceivedPercentage[id]
        });
    });

    const sortedCompetencies = Array.from(competencies.entries())
        .sort((a, b) => b[1].percentage - a[1].percentage);

    const strengths = sortedCompetencies.filter(([_, data]) => data.percentage >= 70);
    const weaknesses = sortedCompetencies.filter(([_, data]) => data.percentage < 50);

    return (
        <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '2rem' }}>
            <h1>Результаты оценки компетенций</h1>

            <div style={{
                display: 'grid',
                gridTemplateColumns: '1fr 2fr',
                gap: '2rem',
                marginBottom: '2rem'
            }}>
                <TotalScoreGauge
                    percentage={calculation.totalReceivedPercentage}
                    referenceTotal={calculation.referenceTotal}
                    receivedTotal={calculation.receivedTotal}
                />

                <div style={{
                    padding: '1rem',
                    backgroundColor: '#f4f4f4',
                    borderRadius: '8px'
                }}>
                    <h3>Ключевые выводы</h3>
                    <div style={{ marginTop: '1rem' }}>
                        <p style={{ color: '#0f62ac', marginBottom: '0.5rem' }}>
                            <strong>✓ Сильные стороны:</strong>
                            {strengths.slice(0, 3).map(([name]) => (
                                <span key={name} style={{ display: 'inline-block', margin: '0.25rem' }}>
                                    {name}
                                </span>
                            ))}
                        </p>
                        <p style={{ color: '#da1e28' }}>
                            <strong>✗ Зоны развития:</strong>
                            {weaknesses.slice(0, 3).map(([name]) => (
                                <span key={name} style={{ display: 'inline-block', margin: '0.25rem' }}>
                                    {name}
                                </span>
                            ))}
                        </p>
                    </div>
                </div>
            </div>

            <div style={{ marginBottom: '1rem' }}>
                <button
                    onClick={() => setViewType('chart')}
                    style={{
                        padding: '0.5rem 1rem',
                        marginRight: '0.5rem',
                        backgroundColor: viewType === 'chart' ? '#0f62ac' : '#f4f4f4',
                        color: viewType === 'chart' ? 'white' : '#161616',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: 'pointer'
                    }}
                >
                    Графики
                </button>
                <button
                    onClick={() => setViewType('list')}
                    style={{
                        padding: '0.5rem 1rem',
                        backgroundColor: viewType === 'list' ? '#0f62ac' : '#f4f4f4',
                        color: viewType === 'list' ? 'white' : '#161616',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: 'pointer'
                    }}
                >
                    Таблица
                </button>
            </div>

            {/* Кастомные табы вместо Carbon Tabs */}
            <div>
                <div style={{ display: 'flex', gap: '1rem', borderBottom: '1px solid #e0e0e0' }}>
                    <button
                        onClick={() => setActiveTab('radar')}
                        style={{
                            padding: '0.75rem 1rem',
                            backgroundColor: 'transparent',
                            border: 'none',
                            borderBottom: activeTab === 'radar' ? '2px solid #0f62ac' : 'none',
                            color: activeTab === 'radar' ? '#0f62ac' : '#161616',
                            cursor: 'pointer',
                            fontWeight: activeTab === 'radar' ? 'bold' : 'normal'
                        }}
                    >
                        Радар
                    </button>
                    <button
                        onClick={() => setActiveTab('detail')}
                        style={{
                            padding: '0.75rem 1rem',
                            backgroundColor: 'transparent',
                            border: 'none',
                            borderBottom: activeTab === 'detail' ? '2px solid #0f62ac' : 'none',
                            color: activeTab === 'detail' ? '#0f62ac' : '#161616',
                            cursor: 'pointer',
                            fontWeight: activeTab === 'detail' ? 'bold' : 'normal'
                        }}
                    >
                        Детальный список
                    </button>
                </div>

                <div style={{ marginTop: '1rem' }}>
                    {activeTab === 'radar' && (
                        <CompetenciesRadar competencies={competencies} />
                    )}
                    {activeTab === 'detail' && (
                        viewType === 'chart' ? (
                            <div>
                                {Array.from(competencies.entries()).map(([name, data]) => (
                                    <CompetencyBar
                                        key={name}
                                        name={name}
                                        reference={data.reference}
                                        received={data.received}
                                        percentage={data.percentage}
                                    />
                                ))}
                            </div>
                        ) : (
                            <TestingResultsView currAssessment={assessment} />
                        )
                    )}
                </div>
            </div>

            <div style={{
                marginTop: '2rem',
                padding: '1.5rem',
                backgroundColor: '#e8f4ff',
                borderRadius: '8px',
                borderLeft: '4px solid #0f62ac'
            }}>
                <h3>📈 Рекомендации по развитию</h3>
                <ul style={{ marginTop: '1rem', lineHeight: '1.6' }}>
                    {weaknesses.map(([name, data]) => (
                        <li key={name}>
                            <strong>{name}</strong>: достигнуто {Math.round(data.percentage)}%
                            от эталона. Рекомендуется пройти дополнительные курсы и тренинги.
                        </li>
                    ))}
                    {weaknesses.length === 0 && (
                        <li>Отличный результат! Все компетенции на высоком уровне.</li>
                    )}
                </ul>
            </div>
        </div>
    );
};