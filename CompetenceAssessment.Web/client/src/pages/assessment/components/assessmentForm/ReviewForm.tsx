// AssessmentReview.tsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Button, Loading, Modal, TextArea, RadioButtonGroup, RadioButton } from '@carbon/react';
import { observer } from 'mobx-react-lite';
import { assessmentStore } from '../../stores/assessmentStore';
import { Assessment, AssessmentTask } from '../../types/assessment.types';
import {UpdateAssessmentDto} from "../../types/assessment.types";

interface ReviewAnswer {
    answer: string;
    score: number;
    comment: string;
}

export const ReviewForm: React.FC = observer(() => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [assessment, setAssessment] = useState<Assessment | null>(null);
    const [answers, setAnswers] = useState<Record<string, ReviewAnswer>>({});
    const [isLoading, setIsLoading] = useState(true);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [overallComment, setOverallComment] = useState('');
    const [isValid, setIsValid] = useState(false);

    useEffect(() => {
        loadAssessment();
    }, [id]);

    const loadAssessment = async () => {
        setIsLoading(true);
        try {
            await assessmentStore.loadAssessmentById(id!);
            const currentAssessment = assessmentStore.currentAssessment;
            if (!currentAssessment) {
                return;
            }
            setAssessment(currentAssessment);

            const initialAnswers: Record<string, ReviewAnswer> = {};
            currentAssessment.template.tasks.forEach((task: AssessmentTask) => {
                const existingResult = currentAssessment.results.find((r: any) => r.taskId == task.id);
                initialAnswers[task.id] = {
                    answer: existingResult?.answer || '',
                    score: existingResult?.score || 0,
                    comment: existingResult?.comment || ''
                };
            });
            setAnswers(initialAnswers);
        } catch (error) {
            console.error('Ошибка загрузки оценки:', error);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        if (!assessment) return;

        // Проверяем, что все ОТКРЫТЫЕ задания оценены
        const openTasks = assessment.template.tasks.filter((task: AssessmentTask) => task.type == '2');
        const allOpenTasksScored = openTasks.every((task: AssessmentTask) => {
            const reviewAnswer = answers[task.id];
            return reviewAnswer && reviewAnswer.score > 0;
        });

        setIsValid(allOpenTasksScored);
    }, [answers, assessment]);

    const handleAnswerChange = (taskId: string, field: keyof ReviewAnswer, value: any) => {
        setAnswers(prev => ({
            ...prev,
            [taskId]: {
                ...prev[taskId],
                [field]: value
            }
        }));
    };

    const handleSubmit = async () => {
        if (!isValid) return;
        setIsModalOpen(true);
    };

    const handleConfirmSubmit = async () => {
        setIsSubmitting(true);

        try {
            const updateData: UpdateAssessmentDto = {
                assessmentId: id!,
                answers: {},
                scores: {},
                comments: {},
                comment: overallComment
            };

            assessment?.template.tasks.forEach((task: AssessmentTask) => {
                const reviewAnswer = answers[task.id];
                updateData.answers[task.id] = reviewAnswer.answer;

                // Оценки отправляем только для открытых вопросов
                if (task.type == '2') {
                    updateData.scores[task.id] = reviewAnswer.score;
                    updateData.comments[task.id] = reviewAnswer.comment;
                }
            });

            await assessmentStore.updateAssessment(updateData);
            setIsModalOpen(false);
            navigate('/assessments');
        } catch (error) {
            console.error('Ошибка при сохранении:', error);
        } finally {
            setIsSubmitting(false);
        }
    };

    // Генерация значений для шкалы
    const getScaleValues = (scale: number): number[] => {
        return Array.from({ length: scale + 1 }, (_: any, i: number) => i);
    };

    if (isLoading) {
        return <Loading description="Загрузка данных оценки..." withOverlay />;
    }

    if (!assessment) {
        return <div>Оценка не найдена</div>;
    }

    const scaleValues = getScaleValues(assessment.template.scale);
    const openTasks = assessment.template.tasks.filter((task: AssessmentTask) => task.type == '2');
    const testTasks = assessment.template.tasks.filter((task: AssessmentTask) => task.type != '2');

    return (
        <div style={{ maxWidth: '900px', margin: '0 auto', padding: '2rem 1rem' }}>
            <div style={{ marginBottom: '2rem' }}>
                <h2>Проверка оценки: {assessment.template.name}</h2>
                <p style={{ color: '#6f6f6f', marginTop: '0.5rem' }}>
                    Аттестуемый: {assessment.candidate.fullName}
                </p>
                <p style={{ color: '#6f6f6f' }}>
                    Тип: {assessment.type === '1' ? 'Тестирование' : assessment.type === '2' ? 'Анкетирование' : 'Оценка 360 градусов'}
                </p>
                <p style={{ color: '#6f6f6f' }}>
                    Шкала оценивания: {assessment.template.scale}-балльная
                </p>
            </div>

            {/* Тестовые задания (только для просмотра) */}
            {testTasks.length > 0 && (
                <>
                    <h3 style={{ marginBottom: '1rem', fontSize: '1.25rem' }}>Тестовые задания</h3>
                    {testTasks.map((task: AssessmentTask, index: number) => {
                        const reviewAnswer = answers[task.id];

                        return (
                            <div
                                key={task.id}
                                style={{
                                    padding: '1.5rem',
                                    marginBottom: '1.5rem',
                                    backgroundColor: '#ffffff',
                                    border: '1px solid #e0e0e0',
                                    borderRadius: '8px',
                                    boxShadow: '0 1px 3px rgba(0,0,0,0.1)'
                                }}
                            >
                                <div style={{ marginBottom: '1rem' }}>
                                    <div style={{
                                        fontSize: '0.875rem',
                                        color: '#6f6f6f',
                                        marginBottom: '0.5rem'
                                    }}>
                                        Задание {index + 1}
                                    </div>
                                    <div style={{
                                        fontWeight: 500,
                                        fontSize: '1rem',
                                        marginBottom: '0.75rem'
                                    }}>
                                        {task.text}
                                    </div>
                                    <div style={{
                                        padding: '0.75rem',
                                        backgroundColor: '#f4f4f4',
                                        borderRadius: '4px'
                                    }}>
                                        <div style={{ fontSize: '0.75rem', color: '#6f6f6f', marginBottom: '0.25rem' }}>
                                            Ответ аттестуемого:
                                        </div>
                                        <div style={{ whiteSpace: 'pre-wrap' }}>
                                            {reviewAnswer?.answer || 'Нет ответа'}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        );
                    })}
                </>
            )}

            {/* Открытые вопросы (с возможностью оценки и комментария) */}
            {openTasks.length > 0 && (
                <>
                    <h3 style={{ marginBottom: '1rem', fontSize: '1.25rem', marginTop: testTasks.length > 0 ? '2rem' : 0 }}>
                        Открытые вопросы
                    </h3>
                    {openTasks.map((task: AssessmentTask, index: number) => {
                        const reviewAnswer = answers[task.id];

                        return (
                            <div
                                key={task.id}
                                style={{
                                    padding: '1.5rem',
                                    marginBottom: '1.5rem',
                                    backgroundColor: '#ffffff',
                                    border: '1px solid #e0e0e0',
                                    borderRadius: '8px',
                                    boxShadow: '0 1px 3px rgba(0,0,0,0.1)'
                                }}
                            >
                                <div style={{ marginBottom: '1rem' }}>
                                    <div style={{
                                        fontSize: '0.875rem',
                                        color: '#6f6f6f',
                                        marginBottom: '0.5rem'
                                    }}>
                                        Задание {index + 1}
                                    </div>
                                    <div style={{
                                        fontWeight: 500,
                                        fontSize: '1rem',
                                        marginBottom: '0.75rem'
                                    }}>
                                        {task.text}
                                    </div>
                                    <div style={{
                                        padding: '0.75rem',
                                        backgroundColor: '#f4f4f4',
                                        borderRadius: '4px',
                                        marginBottom: '1rem'
                                    }}>
                                        <div style={{ fontSize: '0.75rem', color: '#6f6f6f', marginBottom: '0.25rem' }}>
                                            Ответ аттестуемого:
                                        </div>
                                        <div style={{ whiteSpace: 'pre-wrap' }}>
                                            {reviewAnswer?.answer || 'Нет ответа'}
                                        </div>
                                    </div>
                                </div>

                                <div style={{ marginBottom: '1rem' }}>
                                    <label style={{
                                        display: 'block',
                                        marginBottom: '0.5rem',
                                        fontWeight: 500,
                                        fontSize: '0.875rem'
                                    }}>
                                        Оценка <span style={{ color: '#da1e28' }}>*</span>
                                    </label>
                                    <RadioButtonGroup
                                        name={`score-${task.id}`}
                                        valueSelected={reviewAnswer?.score?.toString() || ''}
                                        onChange={(val: string | number | undefined) => handleAnswerChange(task.id, 'score', Number(val))}
                                        orientation="horizontal"
                                        style={{ display: 'flex', flexWrap: 'wrap', gap: '0.5rem' }}
                                    >
                                        {scaleValues.map((num: number) => (
                                            <RadioButton
                                                key={`${task.id}-${num}`}
                                                id={`${task.id}-${num}`}
                                                labelText={num.toString()}
                                                value={num.toString()}
                                            />
                                        ))}
                                    </RadioButtonGroup>
                                </div>

                                <div>
                                    <label style={{
                                        display: 'block',
                                        marginBottom: '0.5rem',
                                        fontWeight: 500,
                                        fontSize: '0.875rem'
                                    }}>
                                        Комментарий к ответу
                                    </label>
                                    <TextArea
                                        id={`comment-${task.id}`}
                                        labelText={""}
                                        value={reviewAnswer?.comment || ''}
                                        onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) => handleAnswerChange(task.id, 'comment', e.target.value)}
                                        rows={3}
                                        placeholder="Напишите комментарий к ответу..."
                                    />
                                </div>
                            </div>
                        );
                    })}
                </>
            )}

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
                    Завершить проверку
                </Button>
            </div>

            <Modal
                open={isModalOpen}
                modalHeading="Завершение проверки"
                primaryButtonText="Завершить"
                secondaryButtonText="Отмена"
                onRequestClose={() => setIsModalOpen(false)}
                onRequestSubmit={handleConfirmSubmit}
                primaryButtonDisabled={isSubmitting}
                size="md"
            >
                <div style={{ marginBottom: '1rem' }}>
                    <p style={{ marginBottom: '1rem' }}>
                        Пожалуйста, оставьте общий отзыв о результатах оценки:
                    </p>
                    <TextArea
                        id="overall-comment"
                        value={overallComment}
                        onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) => setOverallComment(e.target.value)}
                        rows={4}
                        placeholder="Введите общий отзыв..."
                        labelText="Общий отзыв"
                    />
                </div>
            </Modal>
        </div>
    );
});