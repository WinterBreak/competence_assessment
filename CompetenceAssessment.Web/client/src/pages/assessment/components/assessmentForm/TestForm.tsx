import React, { useState, useEffect } from 'react';
import { Button, Loading } from '@carbon/react';
import { TestQuestion } from './TestQuestion';
import { OpenQuestion } from './OpenQuestion';
import { Template } from '../../../admin/templates/types/template.types';

interface TestFormProps {
    template: Template;
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
            const answer = answers[task.taskId];
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

    // Группируем задания по компетенциям
    const tasksByCompetence = template.tasks.reduce((acc, task) => {
        const competenceName = template.competenceModel?.competencies[task.competenceId.toString()] || 'Общие задания';
        if (!acc[competenceName]) {
            acc[competenceName] = [];
        }
        acc[competenceName].push(task);
        return acc;
    }, {} as Record<string, typeof template.tasks>);

    // Определяем тип задания (заглушка - позже будет из БД)
    const isOpenQuestion = (taskText: string): boolean => {
        // Временная логика: если текст задания содержит "расскажите" или "опишите"
        return taskText.toLowerCase().includes('расскажите') ||
            taskText.toLowerCase().includes('опишите') ||
            taskText.toLowerCase().includes('объясните');
    };

    return (
        <div style={{ maxWidth: '800px', margin: '0 auto' }}>
            <div style={{ marginBottom: '2rem' }}>
                <h2>{template.name}</h2>
                <p style={{ color: '#6f6f6f', marginTop: '0.5rem' }}>
                    Тип: Тестирование | Шкала: {template.scale}-балльная
                </p>
            </div>

            {Object.entries(tasksByCompetence).map(([competenceName, tasks]) => (
                <div key={competenceName} style={{ marginBottom: '2rem' }}>
                    <h3 style={{
                        marginBottom: '1rem',
                        paddingBottom: '0.5rem',
                        borderBottom: '2px solid #0f62ac',
                        color: '#0f62ac'
                    }}>
                        {competenceName}
                    </h3>

                    {tasks.map(task => {
                        const isOpen = isOpenQuestion(task.taskText);

                        if (isOpen) {
                            return (
                                <OpenQuestion
                                    key={task.taskId}
                                    taskText={task.taskText}
                                    value={answers[task.taskId] || ''}
                                    onChange={(value) => handleAnswerChange(task.taskId, value)}
                                    required={true}
                                />
                            );
                        }

                        return (
                            <TestQuestion
                                key={task.taskId}
                                taskText={task.taskText}
                                correctAnswer={task.taskText.includes('правильный') ? 'Правильный ответ' : 'Пример ответа'}
                                value={answers[task.taskId] || ''}
                                onChange={(value) => handleAnswerChange(task.taskId, value)}
                                required={true}
                            />
                        );
                    })}
                </div>
            ))}

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