import React, { useState } from 'react';
import { LineChart, Line, XAxis, YAxis, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import {DatePicker, DatePickerInput, Tag} from '@carbon/react';

interface DevelopmentPoint {
    date: Date;
    score: number;
    assessmentName: string;
}

interface CompetenceDevelopmentProps {
    employeeId: string;
    employeeName: string;
    competenceName: string;
    developmentHistory: DevelopmentPoint[];
    targetScore: number;
}

export const CompetenceDevelopmentTracker: React.FC<CompetenceDevelopmentProps> = ({
                                                                                       employeeName,
                                                                                       competenceName,
                                                                                       developmentHistory,
                                                                                       targetScore
                                                                                   }) => {
    const [dateRange, setDateRange] = useState<[Date, Date]>([
        developmentHistory[0]?.date || new Date(),
        developmentHistory[developmentHistory.length - 1]?.date || new Date()
    ]);

    const filteredHistory = developmentHistory.filter(
        point => point.date >= dateRange[0] && point.date <= dateRange[1]
    );

    const chartData = filteredHistory.map(point => ({
        date: point.date.toLocaleDateString(),
        score: point.score,
        assessment: point.assessmentName
    }));

    const getTrend = () => {
        if (filteredHistory.length < 2) return 'Недостаточно данных';
        const first = filteredHistory[0].score;
        const last = filteredHistory[filteredHistory.length - 1].score;
        const difference = last - first;

        if (difference > 5) return '📈 Положительная динамика';
        if (difference < -5) return '📉 Отрицательная динамика';
        return '➡️ Стабильный результат';
    };

    const getProjection = () => {
        if (filteredHistory.length < 2) return null;

        const lastScore = filteredHistory[filteredHistory.length - 1].score;
        const avgImprovement = (filteredHistory[filteredHistory.length - 1].score - filteredHistory[0].score) / filteredHistory.length;
        const assessmentsNeeded = Math.ceil((targetScore - lastScore) / avgImprovement);

        return assessmentsNeeded > 0 ? assessmentsNeeded : 0;
    };

    return (
        <div style={{ padding: '2rem', backgroundColor: 'white', borderRadius: '8px' }}>
            <div style={{ marginBottom: '2rem' }}>
                <h2>Трекер развития: {competenceName}</h2>
                <p>Сотрудник: {employeeName}</p>
                <Tag type={getTrend().includes('📈') ? 'green' : getTrend().includes('📉') ? 'red' : 'gray'}>
                    {getTrend()}
                </Tag>
            </div>

            <div style={{ marginBottom: '2rem' }}>
                <DatePicker datePickerType="range" onChange={(dates) => setDateRange(dates as [Date, Date])}>
                    <DatePickerInput id="start-date" placeholder="мм/дд/гггг" labelText="Дата начала" />
                    <DatePickerInput id="end-date" placeholder="мм/дд/гггг" labelText="Дата окончания" />
                </DatePicker>
            </div>

            <div style={{ height: '400px', marginBottom: '2rem' }}>
                <ResponsiveContainer>
                    <LineChart data={chartData}>
                        <XAxis dataKey="date" />
                        <YAxis domain={[0, 100]} />
                        <Tooltip />
                        <Legend />
                        <Line
                            type="monotone"
                            dataKey="score"
                            stroke="#0f62ac"
                            name="Текущий уровень"
                            strokeWidth={2}
                        />
                        <Line
                            type="monotone"
                            dataKey={() => targetScore}
                            stroke="#da1e28"
                            name="Целевой уровень"
                            strokeDasharray="5 5"
                            strokeWidth={2}
                        />
                    </LineChart>
                </ResponsiveContainer>
            </div>

            {getProjection() !== null && (
                <div style={{
                    padding: '1rem',
                    backgroundColor: '#e8f4ff',
                    borderRadius: '8px',
                    marginTop: '1rem'
                }}>
                    <h4>Прогноз достижения цели</h4>
                    <p>
                        При сохранении текущего темпа развития, целевой уровень будет достигнут через
                        <strong> {getProjection()} </strong>
                        {getProjection() === 1 ? 'оценку' : 'оценок'}.
                    </p>
                    {getProjection()! > 10 && (
                        <p style={{ color: '#da1e28', marginTop: '0.5rem' }}>
                            ⚠️ Рекомендуется усилить программу развития для данной компетенции.
                        </p>
                    )}
                </div>
            )}
        </div>
    );
};