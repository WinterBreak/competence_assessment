import React, { useEffect, useState } from 'react';
import { Modal, Form, TextArea, FormGroup, RadioButtonGroup, RadioButton, Button, TextInput } from '@carbon/react';
import { TrashCan, Add } from '@carbon/react/icons';
import { Answer } from "../types/task.types";

interface TaskModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSubmit: (data: { id?: string; text: string; type: string; answers?: Record<string, boolean> }) => void;
    initialData?: { id: string; text: string; type: string; answers?: Answer[] | null };
    mode?: 'create' | 'edit';
}

interface AnswerRow {
    id: string;
    text: string;
    isCorrect: boolean;
}

export const TaskModal: React.FC<TaskModalProps> = ({
                                                        isOpen,
                                                        onClose,
                                                        onSubmit,
                                                        initialData,
                                                        mode = 'create'
                                                    }) => {
    const [text, setText] = React.useState('');
    const [type, setType] = React.useState<string>('');
    const [answers, setAnswers] = React.useState<AnswerRow[]>([]);
    const [isSubmitting, setIsSubmitting] = React.useState(false);
    
    useEffect(() => {
        if (initialData) {
            setText(initialData.text);
            setType(initialData.type.toString());

            // Преобразуем существующие ответы в формат AnswerRow
            if (initialData.answers && initialData.answers.length > 0) {
                const initialAnswers = initialData.answers.map((answer, index) => ({
                    id: answer.id || `answer-${Date.now()}-${index}`,
                    text: answer.text,
                    isCorrect: answer.isCorrect
                }));
                setAnswers(initialAnswers);
            } else {
                setAnswers([]);
            }
        } else {
            setText('');
            setType('');
            setAnswers([]);
        }
    }, [initialData, isOpen]);

    const handleTypeChange = (value: string) => {
        setType(value);
        if (value != '1') {
            setAnswers([]);
        }
    };

    const handleAddAnswer = () => {
        if (answers.length < 6) {
            const newAnswer: AnswerRow = {
                id: `answer-${Date.now()}-${answers.length}`,
                text: '',
                isCorrect: false
            };
            setAnswers([...answers, newAnswer]);
        }
    };

    const handleAnswerChange = (id: string, field: 'text' | 'isCorrect', value: string | boolean) => {
        setAnswers(answers.map(answer =>
            answer.id == id
                ? { ...answer, [field]: value }
                : answer
        ));
    };

    const handleDeleteAnswer = (id: string) => {
        setAnswers(answers.filter(answer => answer.id !== id));
    };

    const handleSubmit = async () => {
        if (!text.trim()) return;

        setIsSubmitting(true);

        // Формируем answers в нужном формате для DTO
        const answersDto: Record<string, boolean> = {};
        if (type == '1' && answers.length > 0) {
            answers.forEach(answer => {
                if (answer.text.trim()) {
                    answersDto[answer.text] = answer.isCorrect;
                }
            });
        }

        if (mode === 'edit' && initialData?.id) {
            await onSubmit({ id: initialData.id, text, type, answers: answersDto });
        } else {
            await onSubmit({ text, type, answers: answersDto });
        }

        setIsSubmitting(false);

        // Закрываем окно только если это создание
        if (mode === 'create') {
            onClose();
        }
    };

    const isAddAnswerDisabled = answers.length >= 6;
    const showAnswers = type == '1';
    const isSubmitDisabled = !text.trim() || !type || (type == '1' && answers.length == 0) || isSubmitting;

    // Проверяем, есть ли хотя бы один заполненный ответ
    const hasValidAnswers = type == '1' ? answers.some(a => a.text.trim()) : true;
    const finalSubmitDisabled = isSubmitDisabled || (type == '1' && !hasValidAnswers);

    return (
        <Modal
            key={initialData?.id || mode}
            open={isOpen}
            modalHeading={mode === 'create' ? 'Создание задания' : 'Редактирование задания'}
            primaryButtonText={mode === 'create' ? 'Добавить' : 'Сохранить'}
            secondaryButtonText="Отмена"
            onRequestClose={onClose}
            onRequestSubmit={handleSubmit}
            primaryButtonDisabled={finalSubmitDisabled}
            size="lg"
        >
            <Form>
                <FormGroup legendText="">
                    <TextArea
                        id="text"
                        labelText="Текст задания"
                        placeholder="Введите текст..."
                        value={text}
                        onChange={(e) => setText(e.target.value)}
                        required
                        invalidText="Обязательное поле"
                        rows={4}
                    />
                </FormGroup>

                <FormGroup legendText="Тип задания">
                    <RadioButtonGroup
                        name="task-type"
                        value={type}
                        defaultSelected={type}
                        onChange={(selection) => {
                            if (selection !== undefined) {
                                const value = String(selection);
                                handleTypeChange(value);
                            }
                        }}
                        orientation="horizontal"
                    >
                        <RadioButton
                            labelText="Анкета"
                            value="3"
                            id="radio-3"
                            checked={type == '3'}
                        />
                        <RadioButton
                            labelText="Тестовое задание"
                            value="1"
                            id="radio-1"
                            checked={type == '1'}
                        />
                        <RadioButton
                            labelText="Открытый вопрос"
                            value="2"
                            id="radio-2"
                            checked={type == '2'}
                        />
                    </RadioButtonGroup>
                </FormGroup>

                {showAnswers && (
                    <FormGroup legendText="Варианты ответов">
                        <div style={{ marginBottom: '1rem' }}>
                            <Button
                                kind="secondary"
                                size="sm"
                                renderIcon={Add}
                                onClick={handleAddAnswer}
                                disabled={isAddAnswerDisabled}
                                style={{ marginBottom: '1rem' }}
                            >
                                Добавить ответ ({answers.length}/6)
                            </Button>
                        </div>

                        {answers.map((answer, index) => (
                            <div key={answer.id} style={{
                                display: 'flex',
                                gap: '1rem',
                                alignItems: 'flex-end',
                                marginBottom: '1rem',
                                padding: '0.5rem',
                                backgroundColor: '#f4f4f4',
                                borderRadius: '4px'
                            }}>
                                <div style={{ flex: 1 }}>
                                    <TextInput
                                        id={`answer-${answer.id}`}
                                        labelText={`Вариант ответа ${index + 1}`}
                                        placeholder="Введите вариант ответа..."
                                        value={answer.text}
                                        onChange={(e) => handleAnswerChange(answer.id, 'text', e.target.value)}
                                        required={type == '1'}
                                        invalid={answer.text.trim() === ''}
                                        invalidText="Поле обязательно для заполнения"
                                    />
                                </div>
                                <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                                    <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer' }}>
                                        <input
                                            type="checkbox"
                                            checked={answer.isCorrect}
                                            onChange={(e) => handleAnswerChange(answer.id, 'isCorrect', e.target.checked)}
                                            style={{ width: '16px', height: '16px', cursor: 'pointer' }}
                                        />
                                        <span style={{ fontSize: '14px' }}>Верный ответ</span>
                                    </label>
                                    <Button
                                        kind="ghost"
                                        size="sm"
                                        renderIcon={TrashCan}
                                        onClick={() => handleDeleteAnswer(answer.id)}
                                        iconDescription="Удалить ответ"
                                        hasIconOnly
                                    />
                                </div>
                            </div>
                        ))}

                        {answers.length === 0 && (
                            <div style={{
                                padding: '1rem',
                                textAlign: 'center',
                                color: '#6f6f6f',
                                backgroundColor: '#f4f4f4',
                                borderRadius: '4px'
                            }}>
                                Нет добавленных ответов. Нажмите "Добавить ответ" чтобы создать варианты.
                            </div>
                        )}
                    </FormGroup>
                )}
            </Form>
        </Modal>
    );
};