// src/components/AssessmentForm/SurveyForm.tsx
import React, { useState, useEffect } from 'react';
import { Button, Loading } from '@carbon/react';
import { ScaleQuestion } from './ScaleQuestion';
import { OpenQuestion } from './OpenQuestion';
import { Template } from '../../../admin/templates/types/template.types';

interface SurveyFormProps {
    template: Template;
    onSubmit: (answers: Record<string, any>) => void;
    isSubmitting?: boolean;
}

interface Answer {
    taskId: string;
    value: any;
}

export const SurveyForm: React.FC<SurveyFormProps> = ({
                                                                        template,
                                                                        onSubmit,
                                                                        isSubmitting = false
                                                                    }) => {
    const [answers, setAnswers] = useState<Record<string, any>>({});
    const [isValid, setIsValid] = useState(false);

    // Проверка заполнения всех обязательных полей
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

    return (
        <div style={{ maxWidth: '800px', margin: '0 auto' }}>
            <div style={{ marginBottom: '2rem' }}>
                <h2>{template.name}</h2>
                <p style={{ color: '#6f6f6f', marginTop: '0.5rem' }}>
                    Шкала оценивания: {template.scale}-балльная
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
                        // Определяем тип задания по типу шаблона
                        // Для анкеты все задания - это шкала
                        return (
                            <ScaleQuestion
                                key={task.taskId}
                                taskText={task.taskText}
                                scale={template.scale}
                                value={answers[task.taskId] || null}
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
                    {isSubmitting ? 'Отправка...' : 'Завершить оценку'}
                </Button>
            </div>
        </div>
    );
};