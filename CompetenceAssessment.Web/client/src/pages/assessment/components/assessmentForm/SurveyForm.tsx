import React, { useState, useEffect } from 'react';
import { Button, Loading } from '@carbon/react';
import { ScaleQuestion } from './ScaleQuestion';
import { OpenQuestion } from './OpenQuestion';
import {TestQuestion} from "./TestQuestion";
import {AssessmentTask, AssessmentTemplate} from "../../types/assessment.types";

interface SurveyFormProps {
    template: AssessmentTemplate;
    onSubmit: (answers: Record<string, any>) => void;
    isSubmitting?: boolean;
}

export const SurveyForm: React.FC<SurveyFormProps> = ({
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

    const renderQuestion = (task: AssessmentTask) => {
        switch (Number(task.type)) {
            case 1:
                return (
                    <TestQuestion
                        taskId={task.id}
                        taskText={task.text}
                        answers={task.answers}
                        value={answers[task.id] || null}
                        onChange={(value) => handleAnswerChange(task.id, value)}
                        required={true}
                    />
                );
            case 2:
                return (
                    <OpenQuestion
                        taskId={task.id}
                        taskText={task.text}
                        value={answers[task.id] || null}
                        onChange={(value) => handleAnswerChange(task.id, value)}
                        required={true}
                    />
                );
            case 3:
                return (
                    <ScaleQuestion
                        taskId={task.id}
                        taskText={task.text}
                        scale={template.scale}
                        value={answers[task.id] || null}
                        onChange={(value) => handleAnswerChange(task.id, value)}
                        required={true}
                    />
                );
            default:
                return <div>Unknown question type: {task.type}</div>;
        }
    };

    if (!template.tasks) {
        return <Loading description="Загрузка заданий..." />;
    }

    return (
        <div style={{ maxWidth: '800px', margin: '0 auto' }}>
            <div style={{ marginBottom: '2rem' }}>
                <h2>{template.name}</h2>
            </div>

            {template.tasks.map(task => (
                <div key={`${task.id}`} style={{ marginBottom: '2rem' }}>
                    {renderQuestion(task)}
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