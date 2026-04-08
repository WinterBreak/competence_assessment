// src/components/CompetenceModelModal/CompetenceModelModal.tsx
import React, { useState, useEffect } from 'react';
import {
    Modal,
    Form,
    TextArea,
    FormGroup,
    TextInput,
    Button,
    ToastNotification
} from '@carbon/react';
import { Add } from '@carbon/react/icons';
import { CompetenceItem } from './CompetenceItem';
import { SelectCompetenciesModal } from './SelectCompetenciesModal';
import { Competence } from '../../competencies/types/competence.types';
import {ModelCompetence} from "../types/model.types";

interface ModalCompetenceData {
    competenceId: string;
    weight: number;
}

interface CompetenceModelModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSubmit: (data: {
        id?: string;
        name: string;
        description?: string;
        competencies: ModalCompetenceData[]
    }) => void;
    initialData?: {
        id: string;
        name: string;
        description?: string;
        competencies?: ModelCompetence[];
    };
    mode?: 'create' | 'edit';
}

export const CompetenceModelModal: React.FC<CompetenceModelModalProps> = ({
                                                                              isOpen,
                                                                              onClose,
                                                                              onSubmit,
                                                                              initialData,
                                                                              mode = 'create'
                                                                          }) => {
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [competencies, setCompetencies] = useState<ModelCompetence[]>([]);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [isSelectModalOpen, setIsSelectModalOpen] = useState(false);
    const [error, setError] = useState('');

    useEffect(() => {
        if (initialData) {
            setName(initialData.name);
            setDescription(initialData.description ?? '');
            setCompetencies(initialData.competencies ?? []);
        } else {
            setName('');
            setDescription('');
            setCompetencies([]);
        }
    }, [initialData, isOpen]);

    const handleAddCompetencies = (newCompetencies: Competence[]) => {
        const newItems: ModelCompetence[] = newCompetencies.map(comp => ({
            modelId: initialData?.id || '',
            competenceId: comp.id,
            name: comp.name,
            description: comp.description,
            weight: 0.5
        }));
        setCompetencies([...competencies, ...newItems]);
    };

    const handleWeightChange = (competenceId: string, weight: number) => {
        setCompetencies(competencies.map(item =>
            item.competenceId === competenceId ? { ...item, weight } : item
        ));
    };

    const handleRemoveCompetence = (competenceId: string) => {
        setCompetencies(competencies.filter(item => item.competenceId !== competenceId));
    };

    const validate = (): boolean => {
        if (competencies.length === 0) {
            setError('Необходимо добавить хотя бы одну компетенцию');
            return false;
        }
        
        if (competencies.length > 60) {
            setError('Нельзя добавить более 60 компетенций в одну модель');
            return false;
        }
        
        const competenceIds = competencies.map(c => c.competenceId);
        const uniqueIds = new Set(competenceIds);
        if (competenceIds.length !== uniqueIds.size) {
            setError('Компетенции не должны повторяться');
            return false;
        }
        
        const invalidWeight = competencies.find(c => c.weight < 0 || c.weight > 1);
        if (invalidWeight) {
            setError('Веса компетенций должны быть в диапазоне от 0 до 1');
            return false;
        }

        setError('');
        return true;
    };

    const handleSubmit = async () => {
        if (!name.trim()) return;
        if (!validate()) return;

        setIsSubmitting(true);
        
        await onSubmit({
            id: initialData?.id,
            name: name,
            description: description,
            competencies: competencies.map(c => ({
                competenceId: c.competenceId,
                weight: c.weight
            }))
        });

        setIsSubmitting(false);
        onClose();
    };

    const existingCompetenceIds = competencies.map(c => c.competenceId);

    return (
        <>
            <Modal
                open={isOpen}
                modalHeading={mode === 'create' ? 'Создание модели компетенций' : 'Редактирование модели компетенций'}
                primaryButtonText={mode === 'create' ? 'Создать' : 'Сохранить'}
                secondaryButtonText="Отмена"
                onRequestClose={onClose}
                onRequestSubmit={handleSubmit}
                primaryButtonDisabled={!name.trim() || isSubmitting}
                size="lg"
            >
                {error && (
                    <ToastNotification
                        kind="error"
                        title="Ошибка"
                        subtitle={error}
                        onClose={() => setError('')}
                        style={{ marginBottom: '1rem' }}
                        lowContrast
                    />
                )}

                <Form>
                    <FormGroup legendText="">
                        <TextInput
                            id="name"
                            labelText="Название модели"
                            placeholder="Введите название..."
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            required
                            invalidText="Обязательное поле"
                        />
                    </FormGroup>

                    <FormGroup legendText="Описание">
                        <TextArea
                            labelText=""
                            placeholder="Введите описание..."
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            rows={3}
                        />
                    </FormGroup>

                    <FormGroup legendText="Компетенции">
                        <div style={{ marginBottom: '1rem' }}>
                            <Button
                                kind="secondary"
                                onClick={() => setIsSelectModalOpen(true)}
                                renderIcon={Add}
                                size="sm"
                            >
                                Добавить компетенции
                            </Button>
                        </div>

                        <div style={{
                            maxHeight: '400px',
                            overflowY: 'auto',
                            border: '1px solid #e0e0e0',
                            borderRadius: '4px'
                        }}>
                            {competencies.length === 0 ? (
                                <div style={{
                                    textAlign: 'center',
                                    padding: '2rem',
                                    color: '#6f6f6f'
                                }}>
                                    Нет добавленных компетенций. Нажмите "Добавить компетенции"
                                </div>
                            ) : (
                                <>
                                    <div style={{
                                        display: 'flex',
                                        padding: '0.75rem 1rem',
                                        backgroundColor: '#f4f4f4',
                                        fontWeight: 'bold',
                                        borderBottom: '1px solid #e0e0e0'
                                    }}>
                                        <div style={{ flex: 2 }}>Компетенция</div>
                                        <div style={{ width: '120px' }}>Вес (0.0 - 1.0)</div>
                                        <div style={{ width: '50px' }}></div>
                                    </div>
                                    {competencies.map(item => (
                                        <CompetenceItem
                                            key={`${item.modelId}-${item.competenceId}`}
                                            modelId={item.modelId}
                                            competenceId={item.competenceId}
                                            name={item.name}
                                            description={item.description}
                                            weight={item.weight}
                                            onWeightChange={handleWeightChange}
                                            onRemove={handleRemoveCompetence}
                                        />
                                    ))}
                                </>
                            )}
                        </div>

                        {competencies.length > 0 && (
                            <div style={{
                                marginTop: '0.5rem',
                                fontSize: '0.75rem',
                                color: '#6f6f6f'
                            }}>
                                Всего: {competencies.length} компетенций (максимум 60)
                            </div>
                        )}
                    </FormGroup>
                </Form>
            </Modal>

            <SelectCompetenciesModal
                isOpen={isSelectModalOpen}
                onClose={() => setIsSelectModalOpen(false)}
                onAdd={handleAddCompetencies}
                existingCompetenceIds={existingCompetenceIds}
            />
        </>
    );
};