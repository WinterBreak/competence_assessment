import React, { useState, useEffect } from 'react';
import { Button, Loading } from '@carbon/react';
import { TestQuestion } from './TestQuestion';
import { OpenQuestion } from './OpenQuestion';
import { Template } from '../../../admin/templates/types/template.types';
import {AssessmentTemplate} from "../../types/assessment.types";

interface TestFormProps {
    template: AssessmentTemplate;
    onSubmit: (answers: Record<string, any>) => void;
    isSubmitting?: boolean;
}

export const TestForm: React.FC<TestFormProps> = ({
                                                      template,
                                                      onSubmit,
                                                      isSubmitting = false
                                                  }) => {
    const [answers, setAnswers] = useState<Record<string, any>>({});
    const [isValid, setIsValid] = useState(false);

    useEffect(() => {
        if (!template.tasks) {
            setIsValid(false);
            return;
        }

        const allAnswered = template.tasks.every(task => {
            const answer = answers[task.id];
            return answer !== undefined && answer !== null && answer !== '';
        });

        setIsValid(allAnswered);
    }, [answers, template.tasks]);

    const handleAnswerChange = (taskId: string, value: any) => {
        setAnswers(prev => ({
            ...prev,
            [taskId]: value
        }));
    };

    const handleSubmit = () => {
        if (isValid) {
            onSubmit(answers);
        }
    };

    if (!template.tasks) {
        return <Loading description="Загрузка заданий..." />;
    }

    return (
        <div style={{ maxWidth: '800px', margin: '0 auto' }}>
            <div style={{ marginBottom: '2rem' }}>
                <h2>{template.name}</h2>
                <p style={{ color: '#6f6f6f', marginTop: '0.5rem' }}>
                    Тип: Тестирование | Шкала: {template.scale}-балльная
                </p>
            </div>
            
            {template.tasks.map(task => {
                        const isOpen = task.type == '2';

                        if (isOpen) {
                            return (
                                <OpenQuestion
                                    key={task.id}
                                    taskId={task.id}
                                    taskText={task.text}
                                    value={answers[task.id] || ''}
                                    onChange={(value) => handleAnswerChange(task.id, value)}
                                    required={true}
                                />
                            );
                        }

                        return (
                            <TestQuestion
                                key={task.id}
                                taskId={task.id}
                                taskText={task.text}
                                answers={task.answers}
                                value={answers[task.id] || ''}
                                onChange={(value) => handleAnswerChange(task.id, value)}
                                required={true}
                            />
                        );
                    })}

            <div style={{
                display: 'flex',
                justifyContent: 'flex-end',
                marginTop: '2rem',
                paddingTop: '1rem',
                borderTop: '1px solid #e0e0e0'
            }}>
                <Button
                    kind="primary"
                    onClick={handleSubmit}
                    disabled={!isValid || isSubmitting}
                >
                    {isSubmitting ? 'Отправка...' : 'Завершить тестирование'}
                </Button>
            </div>
        </div>
    );
};