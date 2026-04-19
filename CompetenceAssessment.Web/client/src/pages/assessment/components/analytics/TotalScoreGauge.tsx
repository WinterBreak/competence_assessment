import React from 'react';
import { PieChart, Pie, Cell, ResponsiveContainer } from 'recharts';

interface TotalScoreGaugeProps {
    percentage: number;
    referenceTotal: number;
    receivedTotal: number;
}

export const TotalScoreGauge: React.FC<TotalScoreGaugeProps> = ({
                                                                    percentage,
                                                                    referenceTotal,
                                                                    receivedTotal
                                                                }) => {
    const data = [
        { name: 'Достигнуто', value: percentage },
        { name: 'Осталось', value: 100 - percentage }
    ];

    const getColor = (percent: number) => {
        if (percent >= 80) return '#0f62ac';
        if (percent >= 60) return '#ff832b';
        if (percent >= 40) return '#f1c21b';
        return '#da1e28';
    };

    const renderCustomLabel = ({ cx, cy }: any) => {
        return (
            <text
                x={cx}
                y={cy}
                textAnchor="middle"
                dominantBaseline="middle"
                fontSize="24"
                fontWeight="bold"
                fill="#161616"
            >
                {Math.round(percentage)}%
            </text>
        );
    };

    return (
        <div style={{ textAlign: 'center', padding: '1rem' }}>
            <h3>Общий результат</h3>
            <div style={{ position: 'relative', width: '200px', margin: '0 auto' }}>
                <ResponsiveContainer width="100%" height={200}>
                    <PieChart>
                        <Pie
                            data={data}
                            cx="50%"
                            cy="50%"
                            innerRadius={60}
                            outerRadius={80}
                            paddingAngle={0}
                            dataKey="value"
                            startAngle={90}
                            endAngle={-270}
                            label={renderCustomLabel}
                            labelLine={false}
                        >
                            <Cell key="cell-0" fill={getColor(percentage)} />
                            <Cell key="cell-1" fill="#e0e0e0" />
                        </Pie>
                    </PieChart>
                </ResponsiveContainer>
            </div>
            <div style={{ marginTop: '1rem' }}>
                <p>Получено: {receivedTotal} / {referenceTotal} баллов</p>
                <p style={{
                    color: getColor(percentage),
                    fontWeight: 'bold',
                    fontSize: '1.2rem'
                }}>
                    {percentage >= 80 ? 'Отлично!' :
                        percentage >= 60 ? 'Хорошо' :
                            percentage >= 40 ? 'Удовлетворительно' :
                                'Требуется развитие'}
                </p>
            </div>
        </div>
    );
};