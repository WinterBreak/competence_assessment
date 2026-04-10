import React from 'react';
import { TextArea } from '@carbon/react';

interface OpenQuestionProps {
    taskText: string;
    value: string;
    onChange: (value: string) => void;
    required?: boolean;
}

export const OpenQuestion: React.FC<OpenQuestionProps> = ({
                                                              taskText,
                                                              value,
                                                              onChange,
                                                              required = true
                                                          }) => {
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

            <TextArea
                id={`open-${taskText}`}
                labelText="Ваш ответ"
                value={value}
                onChange={(e) => onChange(e.target.value)}
                rows={4}
                required={required}
                placeholder="Введите ваш ответ здесь..."
            />
        </div>
    );
};