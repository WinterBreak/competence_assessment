import React from 'react';
import { RadioButtonGroup, RadioButton } from '@carbon/react';

interface ScaleQuestionProps {
    taskText: string;
    scale: number; // 5 или 10
    value: number | null;
    onChange: (value: number) => void;
    required?: boolean;
}

export const ScaleQuestion: React.FC<ScaleQuestionProps> = ({
                                                                taskText,
                                                                scale,
                                                                value,
                                                                onChange,
                                                                required = true
                                                            }) => {
    const scaleValues = Array.from({ length: scale }, (_, i) => i + 1);

    return (
        <div style={{
            padding: '1.5rem',
            marginBottom: '1rem',
            backgroundColor: '#ffffff',
            border: '1px solid #e0e0e0',
            borderRadius: '8px',
            transition: 'box-shadow 0.2s'
        }}>
            <div style={{ marginBottom: '1rem', fontWeight: 500 }}>
                {taskText}
                {required && <span style={{ color: '#da1e28', marginLeft: '0.25rem' }}>*</span>}
            </div>

            <RadioButtonGroup
                name={`question-${taskText}`}
                valueSelected={value?.toString() || ''}
                onChange={(val) => onChange(Number(val))}
                orientation="horizontal"
                style={{ display: 'flex', flexWrap: 'wrap', gap: '0.5rem' }}
            >
                {scaleValues.map(num => (
                    <RadioButton
                        key={num}
                        id={`scale-${num}`}
                        labelText={num.toString()}
                        value={num.toString()}
                        style={{ marginRight: '0.5rem' }}
                    />
                ))}
            </RadioButtonGroup>
        </div>
    );
};