import React, { useState } from 'react';
import { RadioButtonGroup, RadioButton, TextInput } from '@carbon/react';
import {Answer} from "../../../admin/tasks/types/task.types";

interface TestQuestionProps {
    taskId: string;
    taskText: string;
    answers: Answer[];
    value: string;
    onChange: (value: string) => void;
    required?: boolean;
}

export const TestQuestion: React.FC<TestQuestionProps> = ({
                                                              taskId,
                                                              taskText,
                                                              answers,
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
            
            <RadioButtonGroup // TODO множественный выбор можно сделать, если проверять, сколько isCorrect среди answers
                name={`${taskId}`}
                valueSelected={value}
                onChange={(val) => onChange(val?.toString() || '')}
                orientation="vertical"
            >
                {answers.map((option, index) => (
                    <RadioButton
                        key={`${taskId}-${option.id}`}
                        id={`${taskId}-${option.id}`}
                        labelText={option.text}
                        value={option.text}
                    />
                ))}
            </RadioButtonGroup>
        </div>
    );
};