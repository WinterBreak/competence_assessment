import React from 'react';
import { ProgressIndicator, ProgressStep } from '@carbon/react';

interface CompetencyBarProps {
    name: string;
    reference: number;
    received: number;
    percentage: number;
}

export const CompetencyBar: React.FC<CompetencyBarProps> = ({
                                                                name,
                                                                reference,
                                                                received,
                                                                percentage
                                                            }) => {
    const getStatus = (percent: number) => {
        if (percent >= 80) return 'finished';
        if (percent >= 60) return 'current';
        return 'incomplete';
    };

    const getColor = (percent: number) => {
        if (percent >= 80) return '#0f62ac';
        if (percent >= 60) return '#ff832b';
        if (percent >= 40) return '#f1c21b';
        return '#da1e28';
    };

    return (
        <div style={{
            padding: '1rem',
            marginBottom: '1rem',
            backgroundColor: '#f4f4f4',
            borderRadius: '8px',
            borderLeft: `4px solid ${getColor(percentage)}`
        }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '0.5rem' }}>
                <span style={{ fontWeight: 'bold' }}>{name}</span>
                <span style={{ color: getColor(percentage), fontWeight: 'bold' }}>
                    {Math.round(percentage)}% ({received}/{reference})
                </span>
            </div>
            <div style={{
                width: '100%',
                height: '8px',
                backgroundColor: '#e0e0e0',
                borderRadius: '4px',
                overflow: 'hidden'
            }}>
                <div style={{
                    width: `${percentage}%`,
                    height: '100%',
                    backgroundColor: getColor(percentage),
                    transition: 'width 0.5s ease-in-out'
                }} />
            </div>
            <div style={{ marginTop: '0.5rem', fontSize: '0.875rem', color: '#6f6f6f' }}>
                {percentage >= 80 ? '✓ Компетенция развита отлично' :
                    percentage >= 60 ? 'ℹ Компетенция на хорошем уровне' :
                        percentage >= 40 ? '⚠ Требуется дополнительное развитие' :
                            '✗ Критический уровень - необходимо срочное развитие'}
            </div>
        </div>
    );
};