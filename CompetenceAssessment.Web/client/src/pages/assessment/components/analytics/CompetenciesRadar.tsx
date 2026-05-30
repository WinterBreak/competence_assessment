import React from 'react';
import {
    Radar,
    RadarChart,
    PolarGrid,
    PolarAngleAxis,
    PolarRadiusAxis,
    ResponsiveContainer,
    Legend,
    Tooltip
} from 'recharts';

interface CompetenciesRadarProps {
    competencies: Map<string, { reference: number; received: number; percentage: number }>;
}

export const CompetenciesRadar: React.FC<CompetenciesRadarProps> = ({ competencies }) => {
    const data = Array.from(competencies.entries()).map(([name, values]) => ({
        competence: name,
        // эталон: values.reference,
        получено: (values.received * 100 / values.reference).toFixed(2),
        процент: values.percentage
    }));

    return (
        <div style={{ width: '100%', height: 400, padding: '1rem' }}>
            <h3>Профиль компетенций</h3>
            <ResponsiveContainer>
                <RadarChart data={data}>
                    <PolarGrid />
                    {/* @ts-ignore */}
                    <PolarAngleAxis
                        dataKey="competence"
                        tick={{ fill: '#161616', fontSize: 12 }}
                    />
                    {/* @ts-ignore */}
                    <PolarRadiusAxis
                        domain={[0, 100]}
                        tick={{ fill: '#6f6f6f' }}
                    />
                    {/*<Radar*/}
                    {/*    name="Эталон"*/}
                    {/*    dataKey="эталон"*/}
                    {/*    stroke="#0f62ac"*/}
                    {/*    fill="#0f62ac"*/}
                    {/*    fillOpacity={0.3}*/}
                    {/*/>*/}
                    <Radar
                        name="Получено"
                        dataKey="получено"
                        stroke="#ff832b"
                        fill="#ff832b"
                        fillOpacity={0.3}
                    />
                    <Tooltip />
                    <Legend />
                </RadarChart>
            </ResponsiveContainer>
        </div>
    );
};