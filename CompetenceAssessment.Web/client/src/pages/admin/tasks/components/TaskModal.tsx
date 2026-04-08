import React, { useEffect } from 'react';
import { Modal, Form, TextArea, FormGroup, RadioButtonGroup, RadioButton } from '@carbon/react';

interface TaskModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSubmit: (data: { id?: string; text: string; type: string; answer?: string | null }) => void;
    initialData?: { id: string; text: string; type: string; answer?: string | null };
    mode?: 'create' | 'edit';
}

export const TaskModal: React.FC<TaskModalProps> = ({
                                                                    isOpen,
                                                                    onClose,
                                                                    onSubmit,
                                                                    initialData,
                                                                    mode = 'create'
                                                                }) => {
    const [text, setText] = React.useState('');
    const [type, setType] = React.useState('');
    const [answer, setAnswer] = React.useState('');
    const [isAnswerDisabled, setAnswerDisabled] = React.useState(true);
    const [isSubmitting, setIsSubmitting] = React.useState(false);

    useEffect(() => {
        if (initialData) {
            setText(initialData.text);
            setType(initialData.type);
            setAnswer(initialData.answer ?? '');
        } else {
            setText('');
            setType('');
            setAnswer('');
        }
    }, [initialData, isOpen]);

    const handleTypeChange = (value: string) => {
        setType(value);
        if (value !== '1') {
            setAnswer('');
            setAnswerDisabled(true);
        }
        else {
            setAnswerDisabled(false);
        }
    };


    const handleSubmit = async () => {
        if (!text.trim()) return;

        setIsSubmitting(true);

        if (mode === 'edit' && initialData?.id) {
            await onSubmit({ id: initialData.id, text, type, answer });
        } else {
            await onSubmit({ text, type, answer });
        }

        setIsSubmitting(false);
        onClose();
    };

    return (
        <Modal
            open={isOpen}
            modalHeading={mode === 'create' ? 'Cоздание компетенции' : 'Редактирование компетенции'}
            primaryButtonText={mode === 'create' ? 'Добавить' : 'Сохранить'}
            secondaryButtonText="Отмена"
            onRequestClose={onClose}
            onRequestSubmit={handleSubmit}
            primaryButtonDisabled={!text.trim() || !type || isSubmitting}
            size="sm"
        >
            <Form>

                <FormGroup legendText="">
                    <TextArea
                        id="text"
                        labelText="Тест задания"
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
                        onChange={(selection, name, event) => {
                            if (selection !== undefined) {
                                setType(String(selection));
                                handleTypeChange(String(selection) ?? '0');
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

                <FormGroup legendText="">
                    <TextArea
                        id="description"
                        labelText="Ответ"
                        placeholder="Введите ответ..."
                        value={answer}
                        onChange={(e) => setAnswer(e.target.value)}
                        disabled={isAnswerDisabled}
                        // TODO обработать: если доступно - обязательно
                        rows={4}
                    />
                </FormGroup>
            </Form>
        </Modal>
    );
};