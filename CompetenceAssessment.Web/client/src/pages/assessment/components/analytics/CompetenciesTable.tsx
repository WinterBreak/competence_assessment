import React, { useState } from 'react';
import { DataTable, Table, TableHead, TableRow, TableHeader, TableBody, TableCell } from '@carbon/react';

interface CompetenciesTableProps {
    competencies: Map<string, { reference: number; received: number; percentage: number }>;
}

export const CompetenciesTable: React.FC<CompetenciesTableProps> = ({ competencies }) => {
    const [sortBy, setSortBy] = useState<'name' | 'percentage'>('percentage');
    const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('desc');

    const sortedData = Array.from(competencies.entries())
        .map(([name, values]) => ({
            name,
            ...values,
            gap: values.reference - values.received
        }))
        .sort((a, b) => {
            if (sortBy === 'name') {
                return sortOrder === 'asc'
                    ? a.name.localeCompare(b.name)
                    : b.name.localeCompare(a.name);
            } else {
                return sortOrder === 'asc'
                    ? a.percentage - b.percentage
                    : b.percentage - a.percentage;
            }
        });

    const getPercentageColor = (percent: number) => {
        if (percent >= 80) return '#0f62ac';
        if (percent >= 60) return '#ff832b';
        if (percent >= 40) return '#f1c21b';
        return '#da1e28';
    };

    return (
        <div style={{ padding: '1rem' }}>
            <div style={{ marginBottom: '1rem', display: 'flex', gap: '1rem', justifyContent: 'flex-end' }}>
                <button onClick={() => {
                    setSortBy('name');
                    setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
                }} style={{ padding: '0.5rem', cursor: 'pointer' }}>
                    Сортировать по имени {sortBy === 'name' && (sortOrder === 'asc' ? '↑' : '↓')}
                </button>
                <button onClick={() => {
                    setSortBy('percentage');
                    setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
                }} style={{ padding: '0.5rem', cursor: 'pointer' }}>
                    Сортировать по проценту {sortBy === 'percentage' && (sortOrder === 'asc' ? '↑' : '↓')}
                </button>
            </div>

            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                <thead>
                <tr style={{ backgroundColor: '#f4f4f4', borderBottom: '2px solid #e0e0e0' }}>
                    <th style={{ padding: '1rem', textAlign: 'left' }}>Компетенция</th>
                    <th style={{ padding: '1rem', textAlign: 'center' }}>Эталон</th>
                    <th style={{ padding: '1rem', textAlign: 'center' }}>Получено</th>
                    <th style={{ padding: '1rem', textAlign: 'center' }}>Разрыв</th>
                    <th style={{ padding: '1rem', textAlign: 'center' }}>Процент</th>
                    <th style={{ padding: '1rem', textAlign: 'left' }}>Статус</th>
                </tr>
                </thead>
                <tbody>
                {sortedData.map((item) => (
                    <tr key={item.name} style={{ borderBottom: '1px solid #e0e0e0' }}>
                        <td style={{ padding: '1rem', fontWeight: 'bold' }}>{item.name}</td>
                        <td style={{ padding: '1rem', textAlign: 'center' }}>{item.reference}</td>
                        <td style={{ padding: '1rem', textAlign: 'center' }}>{item.received}</td>
                        <td style={{
                            padding: '1rem',
                            textAlign: 'center',
                            color: item.gap > 0 ? '#da1e28' : '#0f62ac',
                            fontWeight: 'bold'
                        }}>
                            {item.gap > 0 ? `-${item.gap}` : `+${Math.abs(item.gap)}`}
                        </td>
                        <td style={{
                            padding: '1rem',
                            textAlign: 'center',
                            color: getPercentageColor(item.percentage),
                            fontWeight: 'bold'
                        }}>
                            {Math.round(item.percentage)}%
                        </td>
                        <td style={{ padding: '1rem' }}>
                            <div style={{
                                width: '100px',
                                height: '6px',
                                backgroundColor: '#e0e0e0',
                                borderRadius: '3px',
                                overflow: 'hidden'
                            }}>
                                <div style={{
                                    width: `${item.percentage}%`,
                                    height: '100%',
                                    backgroundColor: getPercentageColor(item.percentage)
                                }} />
                            </div>
                        </td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
};