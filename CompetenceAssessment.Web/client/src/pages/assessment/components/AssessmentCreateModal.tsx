// src/components/AssessmentCreateModal/AssessmentCreateModal.tsx
import React, { useState, useEffect } from 'react';
import {
    Modal,
    Form,
    FormGroup,
    RadioButtonGroup,
    RadioButton,
    Button,
    ToastNotification,
    Loading,
    Tag,
} from '@carbon/react';
import { Add } from '@carbon/react/icons';
import { assessmentStore } from '../stores/assessmentStore';
import { templateStore } from '../../admin/templates/stores/templateStore';
import { userStore } from '../../admin/users/stores/userStore';
import { Assessment } from '../types/assessment.types';
import { Template } from '../../admin/templates/types/template.types';
import { User } from '../../admin/users/types/user.types';
import { SelectTemplateModal } from './SelectTemplateModal';
import { SelectUsersModal } from './SelectUserModal';
import { observer } from 'mobx-react-lite';

interface AssessmentCreateModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSubmit: (data: {
        templateId: string;
        type: string;
        candidateId: string;
        inspectorsIds?: string[];
    }) => Promise<void>;
}

export const AssessmentCreateModal: React.FC<AssessmentCreateModalProps> = observer(({
                                                                                         isOpen,
                                                                                         onClose,
                                                                                         onSubmit
                                                                                     }) => {
    const [assessmentType, setAssessmentType] = useState<string>('');
    const [selectedTemplate, setSelectedTemplate] = useState<Template | null>(null);
    const [selectedCandidate, setSelectedCandidate] = useState<User | null>(null);
    const [selectedInspectors, setSelectedInspectors] = useState<User[]>([]);

    const [isSelectTemplateOpen, setIsSelectTemplateOpen] = useState(false);
    const [isSelectCandidateOpen, setIsSelectCandidateOpen] = useState(false);
    const [isSelectInspectorsOpen, setIsSelectInspectorsOpen] = useState(false);

    const [error, setError] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    
    useEffect(() => {
        if (isOpen) {
            setAssessmentType('1');
        }
    }, [isOpen]);
    
    const hasOpenTasks = (template: Template | null): boolean => {
        if (!template) return false;
        if (template.tasks == undefined) return false;
        return template.tasks.find(t => t.type == '2') != undefined;
    };
    
    const showInspectorsSelection = (): boolean => {
        if (!assessmentType || !selectedTemplate) return false;

        if (assessmentType === '3') {
            return true;
        }

        if (assessmentType === '1' && hasOpenTasks(selectedTemplate)) {
            return true;
        }

        return false;
    };
    
    const getUsersModalTitle = (): string => {
        if (assessmentType === '3') {
            return 'Выбор участников оценки (до 10 человек)';
        }
        return 'Выбор проверяющего';
    };
    
    const isMultiSelect = (): boolean => {
        return assessmentType === '3';
    };
    
    const validate = (): boolean => {
        if (!assessmentType) {
            setError('Выберите метод оценки');
            return false;
        }

        if (!selectedTemplate) {
            setError('Выберите шаблон');
            return false;
        }

        if (!selectedCandidate) {
            setError('Выберите аттестуемого');
            return false;
        }

        if (showInspectorsSelection()) {
            if (assessmentType === '3') {
                if (selectedInspectors.length < 2) {
                    setError('Для оценки 360 градусов необходимо выбрать минимум 2 участников');
                    return false;
                }
                if (selectedInspectors.length > 10) {
                    setError('Максимум 10 участников для оценки 360 градусов');
                    return false;
                }
            } else if (selectedInspectors.length === 0) {
                setError('Выберите проверяющего');
                return false;
            }
        }

        setError('');
        return true;
    };

    const handleSubmit = async () => {
        if (!validate()) return;

        setIsSubmitting(true);

        try {
            await onSubmit({
                templateId: selectedTemplate!.id,
                type: assessmentType,
                candidateId: selectedCandidate!.id,
                inspectorsIds: showInspectorsSelection() ? selectedInspectors.map(i => i.id) : []
            });
            
            setAssessmentType('');
            setSelectedTemplate(null);
            setSelectedCandidate(null);
            setSelectedInspectors([]);
            setError('');
            onClose();
        } catch (err) {
            setError('Ошибка при создании оценки');
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleClose = () => {
        setAssessmentType('');
        setSelectedTemplate(null);
        setSelectedCandidate(null);
        setSelectedInspectors([]);
        setError('');
        onClose();
    };

    const getAssessmentTypeLabel = (type: string): string => {
        switch (type) {
            case '1': return 'Тестирование';
            case '2': return 'Анкетирование';
            case '3': return '360 градусов';
            default: return '';
        }
    };

    return (
        <>
            <Modal
                open={isOpen}
                modalHeading="Создание оценки компетенций"
                primaryButtonText="Создать"
                secondaryButtonText="Отмена"
                onRequestClose={handleClose}
                onRequestSubmit={handleSubmit}
                primaryButtonDisabled={isSubmitting}
                size="md"
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

                {isLoading && <Loading description="Загрузка..." />}

                <Form>
                    <FormGroup legendText="Метод оценки">
                        <RadioButtonGroup
                            name="assessment-type"
                            value={assessmentType}
                            onChange={(value) => {
                                setAssessmentType(value?.toString() ?? '');
                                setSelectedTemplate(null);
                                setSelectedInspectors([]);
                            }}
                            orientation="vertical"
                        >
                            <RadioButton
                                labelText="Тестирование"
                                value="1"
                                id="type-1"
                            />
                            <RadioButton
                                labelText="Анкетирование"
                                value="2"
                                id="type-2"
                            />
                            <RadioButton
                                labelText="360 градусов"
                                value="3"
                                id="type-3"
                            />
                        </RadioButtonGroup>
                    </FormGroup>
                    
                    {assessmentType && (
                        <FormGroup legendText="Шаблон">
                            {!selectedTemplate ? (
                                <Button
                                    kind="secondary"
                                    onClick={() => setIsSelectTemplateOpen(true)}
                                    renderIcon={Add}
                                >
                                    Выбрать шаблон
                                </Button>
                            ) : (
                                <div style={{
                                    padding: '1rem',
                                    backgroundColor: '#f4f4f4',
                                    borderRadius: '4px'
                                }}>
                                    <div style={{ fontWeight: 'bold' }}>{selectedTemplate.name}</div>
                                    <div style={{ fontSize: '0.75rem', color: '#6f6f6f', marginTop: '0.25rem' }}>
                                        Тип: {selectedTemplate.type == '1' ? 'Тест' : 'Анкета'} |
                                        Шкала: {selectedTemplate.scale}-балльная
                                    </div>
                                    <Button
                                        kind="ghost"
                                        size="sm"
                                        onClick={() => setSelectedTemplate(null)}
                                        style={{ marginTop: '0.5rem' }}
                                    >
                                        Изменить
                                    </Button>
                                </div>
                            )}
                        </FormGroup>
                    )}

                    {/* Выбор аттестуемого */}
                    {assessmentType && (
                        <FormGroup legendText="Аттестуемый">
                            {!selectedCandidate ? (
                                <Button
                                    kind="secondary"
                                    onClick={() => setIsSelectCandidateOpen(true)}
                                    renderIcon={Add}
                                >
                                    Выбрать аттестуемого
                                </Button>
                            ) : (
                                <div style={{
                                    padding: '1rem',
                                    backgroundColor: '#f4f4f4',
                                    borderRadius: '4px'
                                }}>
                                    <div style={{ fontWeight: 'bold' }}>
                                        {selectedCandidate.lastName} {selectedCandidate.firstName} {selectedCandidate.secondName}
                                    </div>
                                    <div style={{ fontSize: '0.75rem', color: '#6f6f6f', marginTop: '0.25rem' }}>
                                        {selectedCandidate.position} | {selectedCandidate.department}
                                    </div>
                                    <Button
                                        kind="ghost"
                                        size="sm"
                                        onClick={() => setSelectedCandidate(null)}
                                        style={{ marginTop: '0.5rem' }}
                                    >
                                        Изменить
                                    </Button>
                                </div>
                            )}
                        </FormGroup>
                    )}
                    
                    {showInspectorsSelection() && selectedCandidate && selectedTemplate && (
                        <FormGroup legendText={assessmentType === '3' ? 'Участники оценки' : 'Проверяющий'}>
                            <div style={{ marginBottom: '0.5rem' }}>
                                {selectedInspectors.length > 0 && (
                                    <div style={{ display: 'flex', flexWrap: 'wrap', gap: '0.5rem', marginBottom: '0.5rem' }}>
                                        {selectedInspectors.map(inspector => (
                                            <Tag key={inspector.id} type="blue">
                                                {inspector.lastName} {inspector.firstName}
                                            </Tag>
                                        ))}
                                    </div>
                                )}
                            </div>

                            <Button
                                kind="secondary"
                                onClick={() => setIsSelectInspectorsOpen(true)}
                                renderIcon={Add}
                            >
                                {selectedInspectors.length > 0 ? 'Изменить выбор' : 'Выбрать'}
                            </Button>

                            {assessmentType === '3' && (
                                <div style={{ fontSize: '0.75rem', color: '#6f6f6f', marginTop: '0.5rem' }}>
                                    Выбрано: {selectedInspectors.length} / 10 (минимум 2)
                                </div>
                            )}
                        </FormGroup>
                    )}
                </Form>
            </Modal>
            
            <SelectTemplateModal
                isOpen={isSelectTemplateOpen}
                onClose={() => setIsSelectTemplateOpen(false)}
                onSelect={setSelectedTemplate}
                assessmentType={assessmentType}
            />
            
            <SelectUsersModal
                isOpen={isSelectCandidateOpen}
                onClose={() => setIsSelectCandidateOpen(false)}
                onSelect={(users) => setSelectedCandidate(users[0] || null)}
                excludeUserIds={[]}
                multiSelect={false}
                title="Выбор аттестуемого"
            />
            
            {showInspectorsSelection() && (
                <SelectUsersModal
                    isOpen={isSelectInspectorsOpen}
                    onClose={() => setIsSelectInspectorsOpen(false)}
                    onSelect={setSelectedInspectors}
                    excludeUserIds={selectedCandidate ? [selectedCandidate.id] : []}
                    multiSelect={isMultiSelect()}
                    title={getUsersModalTitle()}
                    maxSelect={assessmentType == '3' ? 10 : undefined}
                />
            )}
        </>
    );
});