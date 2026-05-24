import React, { useState, useEffect } from 'react';
import {
    Accordion,
    AccordionItem,
    Tag,
    Modal,
    Button,
    Loading
} from '@carbon/react';
import { AssessmentResult, AssessmentTask, Assessment } from '../../types/assessment.types';
import { assessmentStore } from '../../stores/assessmentStore';

interface TestingResultsViewProps {
    currAssessment: Assessment;
}

export const TestingResultsView: React.FC<TestingResultsViewProps> = ({ currAssessment }) => {
    const [loading, setLoading] = useState(true);
    const [assessment, setAssessment] = useState<Assessment | null>(null);
    const [selectedResult, setSelectedResult] = useState<AssessmentResult | null>(null);
    const [isModalOpen, setIsModalOpen] = useState(false);

    useEffect(() => {
        loadAssessmentData();
    }, [assessment]);

    const loadAssessmentData = async () => {
        try {
            setLoading(true);
            if (!currAssessment){
                return;
            }
            setAssessment(currAssessment);
        } catch (error) {
            console.error('Error loading assessment:', error);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return <Loading description="Загрузка результатов..." />;
    }

    if (!assessment) {
        return <div>Данные не найдены</div>;
    }

    // Убираем фильтрацию, берем все задания (и тестовые, и открытые)
    const allTasks = assessment.template.tasks;

    // Создаем мапу результатов по taskId для быстрого доступа
    const resultsMap = new Map<string, AssessmentResult>();
    assessment.results.forEach(result => {
        resultsMap.set(result.taskId, result);
    });

    const getScoreColor = (score: number, maxScore: number = 100) => {
        const percentage = (score / maxScore) * 100;
        if (percentage >= 80) return '#0f62ac';
        if (percentage >= 60) return '#ff832b';
        return '#da1e28';
    };

    const renderTestQuestion = (task: AssessmentTask, result: AssessmentResult) => {
        const userAnswer = result.answer;
        // Ищем правильный ответ в списке ответов задания
        const correctAnswer = task.answers.find(a => a.isCorrect);
        const isCorrect = correctAnswer?.text == userAnswer;

        if (isCorrect) {
            return (
                <div style={{ marginTop: '0.75rem' }}>
                    <div style={{ color: '#0f62ac', fontWeight: 500, marginBottom: '0.5rem' }}>
                        ✓ Дан правильный ответ:
                    </div>
                    <div style={{
                        backgroundColor: '#e6f3e6',
                        padding: '0.75rem',
                        borderRadius: '4px',
                        color: '#0f62ac',
                        borderLeft: '3px solid #0f62ac'
                    }}>
                        {userAnswer}
                    </div>
                </div>
            );
        } else {
            return (
                <div style={{ marginTop: '0.75rem' }}>
                    <div style={{ marginBottom: '0.5rem' }}>
                        <div style={{ color: '#da1e28', fontWeight: 500, marginBottom: '0.25rem' }}>
                            ✗ Дан неправильный ответ:
                        </div>
                        <div style={{
                            backgroundColor: '#ffe6e6',
                            padding: '0.75rem',
                            borderRadius: '4px',
                            color: '#da1e28',
                            borderLeft: '3px solid #da1e28'
                        }}>
                            {userAnswer}
                        </div>
                    </div>
                    {correctAnswer && (
                        <div>
                            <div style={{ color: '#0f62ac', fontWeight: 500, marginBottom: '0.25rem' }}>
                                ✓ Правильный ответ:
                            </div>
                            <div style={{
                                backgroundColor: '#e6f3e6',
                                padding: '0.75rem',
                                borderRadius: '4px',
                                color: '#0f62ac',
                                borderLeft: '3px solid #0f62ac'
                            }}>
                                {correctAnswer.text}
                            </div>
                        </div>
                    )}
                </div>
            );
        }
    };

    const renderOpenQuestion = (task: AssessmentTask, result: AssessmentResult) => {
        return (
            <div style={{ marginTop: '0.75rem' }}>
                <div style={{
                    display: 'flex',
                    alignItems: 'center',
                    gap: '1rem',
                    marginBottom: '1rem',
                    padding: '0.5rem',
                    backgroundColor: '#f4f4f4',
                    borderRadius: '4px'
                }}>
                    <div>
                        <span style={{ fontWeight: 500 }}>Оценка: </span>
                        <span style={{
                            fontWeight: 'bold',
                            color: getScoreColor(result.score)
                        }}>
                            {result.score} баллов
                        </span>
                    </div>
                </div>

                <div style={{ marginBottom: '1rem' }}>
                    <div style={{ fontWeight: 500, marginBottom: '0.25rem' }}>Ответ студента:</div>
                    <div style={{
                        padding: '0.75rem',
                        backgroundColor: '#f4f4f4',
                        borderRadius: '4px',
                        whiteSpace: 'pre-wrap'
                    }}>
                        {result.answer || <em>Ответ не предоставлен</em>}
                    </div>
                </div>

                {result.comment && (
                    <div>
                        <div style={{ fontWeight: 500, marginBottom: '0.25rem' }}>Комментарий эксперта:</div>
                        <div style={{
                            padding: '0.75rem',
                            backgroundColor: '#e8f4ff',
                            borderRadius: '4px',
                            color: '#0043ce',
                            whiteSpace: 'pre-wrap'
                        }}>
                            {result.comment}
                        </div>
                    </div>
                )}
            </div>
        );
    };

    const getQuestionTypeTag = (type: string) => {
        if (type == '1') {
            return <Tag type="blue">Тестовое задание</Tag>;
        }
        return <Tag type="purple">Открытый вопрос</Tag>;
    };

    // Находим общий комментарий (если есть в результатах с комментарием без привязки к заданию)
    const overallComment = assessment.results.find(r => !r.taskId && r.comment)?.comment;

    // Берем все задания, где есть результаты
    const questionsWithResults = allTasks
        .map(task => ({
            task,
            result: resultsMap.get(task.id)
        }))
        .filter(item => item.result);

    // Подсчет количества правильно решенных тестовых заданий
    const correctTestAnswersCount = questionsWithResults.filter(({ task, result }) => {
        if (task.type == '1') {
            const correctAnswer = task.answers.find(a => a.isCorrect);
            return correctAnswer?.text == result?.answer;
        }
        return false;
    }).length;

    const totalTestTasksCount = questionsWithResults.filter(({ task }) => task.type == '1').length;

    return (
        <div style={{ padding: '1rem' }}>
            {/* Заголовок */}
            <div style={{
                marginBottom: '2rem',
                paddingBottom: '1rem',
                borderBottom: '2px solid #e0e0e0'
            }}>
                <h2 style={{ marginBottom: '0.5rem' }}>{assessment.template.name}</h2>

                {/* Подсчет количества правильно решенных тестовых заданий */}
                {totalTestTasksCount > 0 && (
                    <div style={{
                        marginTop: '0.75rem',
                        padding: '0.75rem',
                        backgroundColor: '#f4f4f4',
                        borderRadius: '4px'
                    }}>
                        <strong>Результат тестовой части:</strong>{' '}
                        <span style={{
                            fontSize: '1.25rem',
                            fontWeight: 'bold',
                            color: getScoreColor(
                                (correctTestAnswersCount / totalTestTasksCount) * 100,
                                100
                            )
                        }}>
                            {correctTestAnswersCount} / {totalTestTasksCount} правильных ответов
                        </span>
                    </div>
                )}
            </div>

            {/* Вопросы в аккордеоне */}
            {questionsWithResults.length == 0 ? (
                <div style={{ textAlign: 'center', padding: '3rem', color: '#525252' }}>
                    Нет данных о заданиях
                </div>
            ) : (
                <Accordion>
                    {questionsWithResults.map(({ task, result }, index) => (
                        <AccordionItem
                            key={task.id}
                            title={
                                <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
                                    <span style={{ fontWeight: 'bold' }}>Вопрос {index + 1}</span>
                                    {getQuestionTypeTag(task.type)}
                                    {task.type == '1' && (
                                        <Tag type={result!.answer == task.answers.find(a => a.isCorrect)?.text ? 'green' : 'red'}>
                                            {result!.answer == task.answers.find(a => a.isCorrect)?.text ? 'Верно' : 'Неверно'}
                                        </Tag>
                                    )}
                                </div>
                            }
                        >
                            <div style={{ padding: '0.5rem 0' }}>
                                <div style={{
                                    fontWeight: 500,
                                    marginBottom: '0.75rem',
                                    fontSize: '1rem'
                                }}>
                                    {task.text}
                                </div>

                                {task.type == '1'
                                    ? renderTestQuestion(task, result!)
                                    : renderOpenQuestion(task, result!)
                                }

                                {task.type != '1' && result!.comment && (
                                    <div style={{ marginTop: '1rem' }}>
                                        <Button
                                            kind="ghost"
                                            size="sm"
                                            onClick={() => {
                                                setSelectedResult(result!);
                                                setIsModalOpen(true);
                                            }}
                                        >
                                            Посмотреть комментарий эксперта
                                        </Button>
                                    </div>
                                )}
                            </div>
                        </AccordionItem>
                    ))}
                </Accordion>
            )}

            {/* Общий комментарий */}
            {overallComment && (
                <div style={{
                    marginTop: '2rem',
                    padding: '1rem',
                    backgroundColor: '#e8f4ff',
                    borderRadius: '8px',
                    borderLeft: '4px solid #0f62ac'
                }}>
                    <h4 style={{ marginBottom: '0.5rem' }}>Общий комментарий по тестированию</h4>
                    <div style={{ whiteSpace: 'pre-wrap' }}>{overallComment}</div>
                </div>
            )}

            {/* Модальное окно для комментария эксперта */}
            {selectedResult && (
                <Modal
                    open={isModalOpen}
                    modalHeading="Комментарий эксперта"
                    primaryButtonText="Закрыть"
                    onRequestClose={() => setIsModalOpen(false)}
                    onRequestSubmit={() => setIsModalOpen(false)}
                >
                    <div style={{ padding: '1rem 0' }}>
                        <div style={{ fontWeight: 500, marginBottom: '0.5rem' }}>Задание:</div>
                        <div style={{ marginBottom: '1rem', color: '#525252' }}>
                            {assessment?.template.tasks.find(t => t.id == selectedResult.taskId)?.text}
                        </div>
                        <div style={{ fontWeight: 500, marginBottom: '0.5rem' }}>Ответ студента:</div>
                        <div style={{
                            marginBottom: '1rem',
                            padding: '0.5rem',
                            backgroundColor: '#f4f4f4',
                            borderRadius: '4px'
                        }}>
                            {selectedResult.answer}
                        </div>
                        <div style={{ fontWeight: 500, marginBottom: '0.5rem' }}>Комментарий эксперта:</div>
                        <div style={{
                            padding: '0.5rem',
                            backgroundColor: '#e8f4ff',
                            borderRadius: '4px',
                            whiteSpace: 'pre-wrap'
                        }}>
                            {selectedResult.comment}
                        </div>
                    </div>
                </Modal>
            )}
        </div>
    );
};