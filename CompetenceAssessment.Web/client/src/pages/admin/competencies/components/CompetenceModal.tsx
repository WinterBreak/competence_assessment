import React, { useEffect } from 'react';
import { Modal, Form, TextInput, TextArea, FormGroup } from '@carbon/react';

interface CompetenceModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSubmit: (data: { id?: string; name: string; description?: string }) => void;
    initialData?: { id: string; name: string; description?: string };
    mode?: 'create' | 'edit';
}

export const CompetenceModal: React.FC<CompetenceModalProps> = ({
                                                                    isOpen,
                                                                    onClose,
                                                                    onSubmit,
                                                                    initialData,
                                                                    mode = 'create'
                                                                }) => {
    const [name, setName] = React.useState('');
    const [description, setDescription] = React.useState('');
    const [isSubmitting, setIsSubmitting] = React.useState(false);

    useEffect(() => {
        if (initialData) {
            setName(initialData.name);
            setDescription(initialData.description ?? '');
        } else {
            setName('');
            setDescription('');
        }
    }, [initialData, isOpen]);

    const handleSubmit = async () => {
        if (!name.trim()) return;

        setIsSubmitting(true);

        if (mode === 'edit' && initialData?.id) {
            await onSubmit({ id: initialData.id, name, description });
        } else {
            await onSubmit({ name, description });
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
            primaryButtonDisabled={!name.trim() || isSubmitting}
            size="sm"
        >
            <Form>
                <TextInput
                    id="name"
                    labelText="Название компетенции"
                    placeholder="Введите название..."
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    required
                    invalidText="Обязательное поле"
                />
                
                <FormGroup legendText="">
                    <TextArea
                        id="description"
                        labelText="Описание"
                        placeholder="Введите описание..."
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        rows={4}
                    />
                </FormGroup>
            </Form>
        </Modal>
    );
};