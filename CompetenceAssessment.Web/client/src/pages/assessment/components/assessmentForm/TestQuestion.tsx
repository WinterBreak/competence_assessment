import React, { useState } from 'react';
import { RadioButtonGroup, RadioButton, TextInput } from '@carbon/react';

interface TestQuestionProps {
    taskText: string;
    correctAnswer: string;
    value: string;
    onChange: (value: string) => void;
    required?: boolean;
}

export const TestQuestion: React.FC<TestQuestionProps> = ({
                                                              taskText,
                                                              correctAnswer,
                                                              value,
                                                              onChange,
                                                              required = true
                                                          }) => {
    const [options] = useState([
        correctAnswer,
        `Вариант 1 (неправильный)`,
        `Вариант 2 (неправильный)`,
        `Вариант 3 (неправильный)`
    ]);

    return (
        <div style={{
            padding: '1.5rem',
            marginBottom: '1rem',
            backgroundColor: '#ffffff',
            border: '1px solid #e0e0e0',
            borderRadius: '8px'
        }}>
            <div style={{ marginBottom: '1rem', fontWeight: 500 }}>
                {taskText}
                {required && <span style={{ color: '#da1e28', marginLeft: '0.25rem' }}>*</span>}
            </div>

            <RadioButtonGroup
                name={`test-${taskText}`}
                valueSelected={value}
                onChange={(val) => onChange(val?.toString() || '')}
                orientation="vertical"
            >
                {options.map((option, index) => (
                    <RadioButton
                        key={index}
                        id={`option-${index}`}
                        labelText={option}
                        value={option}
                    />
                ))}
            </RadioButtonGroup>
        </div>
    );
};