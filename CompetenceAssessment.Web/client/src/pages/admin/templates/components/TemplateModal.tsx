// src/components/TemplateModal/TemplateModal.tsx
import React, { useState, useEffect } from 'react';
import {
    Modal,
    Form,
    TextArea,
    FormGroup,
    TextInput,
    Button,
    ToastNotification,
    RadioButtonGroup,
    RadioButton,
} from '@carbon/react';
import { Add, TrashCan } from '@carbon/react/icons';
import { TaskItem } from './TaskItem';
import { SelectTasksModal } from './SelectTasksModal';
import { SelectCompetenceModelsModal } from './SelectCompetenceModelModal';
import { Task } from '../../tasks/types/task.types';
import { CompetenceModel } from '../../competendeModels/types/model.types';
import {TemplateCompetenceModel, TemplateWeight} from "../types/template.types";

interface ModalTaskData {
    taskId: string;
    weight: number;
    competenceId: string;
}

interface TemplateModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSubmit: (data: {
        id?: string;
        name: string;
        description?: string;
        type: string;
        scale: number;
        competenceModelId?: string;
        tasks: ModalTaskData[]
    }) => void;
    initialData?: {
        id: string;
        name: string;
        description?: string;
        type: string;
        scale: number;
        competenceModel?: TemplateCompetenceModel;
        tasks?: TemplateWeight[];
    };
    mode?: 'create' | 'edit';
}

export const TemplateModal: React.FC<TemplateModalProps> = ({
                                                                isOpen,
                                                                onClose,
                                                                onSubmit,
                                                                initialData,
                                                                mode = 'create'
                                                            }) => {
    const [name, setName] = useState('');
    const [type, setType] = useState<string>('');
    const [scale, setScale] = useState<number>(5);
    const [selectedModel, setSelectedModel] = useState<TemplateCompetenceModel | null>(null);
    const [tasks, setTasks] = useState<TemplateWeight[]>([]);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [isSelectModelOpen, setIsSelectModelOpen] = useState(false);
    const [isSelectTasksOpen, setIsSelectTasksOpen] = useState(false);
    const [currentCompetenceId, setCurrentCompetenceId] = useState<string>('');
    const [error, setError] = useState('');

    useEffect(() => {
        if (initialData) {
            setName(initialData.name);
            setType(initialData.type || '');
            setScale(initialData.scale);
            setTasks(initialData.tasks || []);
            setSelectedModel(initialData.competenceModel || null);
        } else {
            setName('');
            setType('');
            setScale(5);
            setTasks([]);
            setSelectedModel(null);
        }
    }, [initialData, isOpen]);

    const handleSelectModel = (models: CompetenceModel[]) => {
        if (models.length > 0) {
            const model = models[0];
            const competencies: Record<string, string> = {};
            model.competencies.forEach(c => {
                competencies[c.competenceId] = c.name;
            });
            const selectedModel: TemplateCompetenceModel = {
                modelId: model.id,
                name: model.name,
                description: model.description,
                competencies: competencies
            };
            setSelectedModel(selectedModel);
            setTasks([]);
        }
    };

    const handleRemoveModel = () => {
        setSelectedModel(null);
        setTasks([]);
    };

    const handleAddTasks = (newTasks: Task[]) => {
        const newItems: TemplateWeight[] = newTasks.map(task => ({
            templateId: initialData?.id || '',
            taskId: task.id,
            type: task.type,
            taskText: task.text,
            competenceId: currentCompetenceId,
            weight: 0.5
        }));
        setTasks([...tasks, ...newItems]);
    };

    const handleWeightChange = (taskId: string, weight: number) => {
        setTasks(tasks.map(item =>
            item.taskId === taskId ? { ...item, weight } : item
        ));
    };

    const handleRemoveTask = (taskId: string) => {
        setTasks(tasks.filter(item => item.taskId !== taskId));
    };

    const openTaskSelection = (competenceId: string) => {
        setCurrentCompetenceId(competenceId);
        setIsSelectTasksOpen(true);
    };
    
    const tasksByCompetence = tasks.reduce((acc, task) => {
        if (!acc[task.competenceId]) {
            const competenceName = selectedModel?.competencies[task.competenceId];
            acc[task.competenceId] = {
                competenceName: competenceName || 'Неизвестная компетенция',
                tasks: []
            };
        }
        acc[task.competenceId].tasks.push(task);
        return acc;
    }, {} as Record<string, { competenceName: string; tasks: TemplateWeight[] }>);

    const validate = (): boolean => {
        if (!selectedModel) {
            setError('Необходимо выбрать модель компетенций');
            return false;
        }
        
        if (tasks.length < 5) {
            setError('Необходимо добавить хотя бы пять заданий');
            return false;
        }
        
        if (tasks.length > 60) {
            setError('Нельзя добавить более 60 заданий в один шаблон');
            return false;
        }
        
        const taskIds = tasks.map(c => c.taskId);
        const uniqueIds = new Set(taskIds);
        if (taskIds.length !== uniqueIds.size) {
            setError('Задания не должны повторяться');
            return false;
        }
        
        if (selectedModel) {
            const modelCompetenceIds = Object.keys(selectedModel.competencies);
            const tasksCompetenceIds = new Set(tasks.map(t => t.competenceId.toString()));
            const missingCompetences = modelCompetenceIds.filter(id => !tasksCompetenceIds.has(id));
            if (missingCompetences.length > 0) {
                setError('Для каждой компетенции из модели должно быть выбрано хотя бы одно задание');
                return false;
            }
        }
        
        const invalidWeight = tasks.find(c => c.weight < 0 || c.weight > 1);
        if (invalidWeight) {
            setError('Веса заданий должны быть в диапазоне от 0 до 1');
            return false;
        }

        setError('');
        return true;
    };

    const handleSubmit = async () => {
        if (!name.trim()) {
            setError('Поле "Название" обязательно для заполнения');
            return;
        }
        if (!validate()) return;

        setIsSubmitting(true);

        await onSubmit({
            id: initialData?.id,
            name: name,
            type: type,
            scale: scale,
            competenceModelId: selectedModel?.modelId,
            tasks: tasks.map(t => ({
                taskId: t.taskId,
                weight: t.weight,
                competenceId: t.competenceId
            }))
        });

        setIsSubmitting(false);
        onClose();
    };

    const existingTaskIds = tasks.map(t => t.taskId);

    return (
        <>
            <Modal
                open={isOpen}
                modalHeading={mode === 'create' ? 'Создание шаблона' : 'Редактирование шаблона'}
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
                            labelText="Название шаблона"
                            placeholder="Введите название..."
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            required
                            invalidText="Обязательное поле"
                        />
                    </FormGroup>

                    <FormGroup legendText="Тип шаблона">
                        <RadioButtonGroup
                            name="template-type"
                            value={type}
                            onChange={(value) => setType(value?.toString() ?? '')}
                            orientation="horizontal"
                        >
                            <RadioButton labelText="Тестирование" value="1" id="type-1" checked={type == '1'}/>
                            <RadioButton labelText="Анкета" value="2" id="type-2" checked={type == '2'} />
                        </RadioButtonGroup>
                    </FormGroup>

                    <FormGroup legendText="Шкала оценивания">
                        <RadioButtonGroup
                            name="template-scale"
                            value={String(scale)}
                            onChange={(value) => setScale(Number(value))}
                            orientation="horizontal"
                        >
                            <RadioButton labelText="5-балльная" value="5" id="scale-5" checked={scale == 5} />
                            <RadioButton labelText="10-балльная" value="10" id="scale-10" checked={scale == 10} />
                        </RadioButtonGroup>
                    </FormGroup>

                    <FormGroup legendText="Модель компетенций">
                        {!selectedModel ? (
                            <Button
                                kind="secondary"
                                onClick={() => setIsSelectModelOpen(true)}
                                renderIcon={Add}
                                size="sm"
                            >
                                Выбрать модель
                            </Button>
                        ) : (
                            <div style={{
                                display: 'flex',
                                justifyContent: 'space-between',
                                alignItems: 'center',
                                padding: '1rem',
                                backgroundColor: '#f4f4f4',
                                borderRadius: '4px'
                            }}>
                                <div>
                                    <div style={{ fontWeight: 'bold' }}>{selectedModel.name}</div>
                                    <div style={{ fontSize: '0.75rem', color: '#6f6f6f' }}>
                                        Компетенций: {Object.keys(selectedModel.competencies)?.length}
                                    </div>
                                </div>
                                <Button
                                    kind="ghost"
                                    size="sm"
                                    onClick={handleRemoveModel}
                                    renderIcon={TrashCan}
                                    iconDescription="Удалить модель"
                                    hasIconOnly
                                />
                            </div>
                        )}
                    </FormGroup>

                    {selectedModel && Object.keys(selectedModel.competencies)?.length > 0 && (
                        <FormGroup legendText="Задания">
                            <div style={{
                                maxHeight: '400px',
                                overflowY: 'auto',
                                border: '1px solid #e0e0e0',
                                borderRadius: '4px'
                            }}>
                                {Object.entries(selectedModel.competencies).map(([id, name]) => (
                                    <div key={id} style={{ marginBottom: '1rem' }}>
                                        <div style={{
                                            display: 'flex',
                                            justifyContent: 'space-between',
                                            alignItems: 'center',
                                            padding: '0.75rem 1rem',
                                            backgroundColor: '#e8f2ff',
                                            borderBottom: '1px solid #e0e0e0'
                                        }}>
                                            <div style={{ fontWeight: 'bold' }}>{name}</div>
                                            <Button
                                                kind="ghost"
                                                size="sm"
                                                onClick={() => openTaskSelection(id)}
                                                renderIcon={Add}
                                                iconDescription="Добавить задание"
                                                hasIconOnly
                                            />
                                        </div>
                                        {tasks.filter(t => t.competenceId.toString() === id).map(task => (
                                            <TaskItem
                                                key={`${task.templateId}-${task.taskId}`}
                                                templateId={task.templateId}
                                                competenceId={task.competenceId}
                                                taskId={task.taskId}
                                                taskText={task.taskText}
                                                weight={task.weight}
                                                onWeightChange={handleWeightChange}
                                                onRemove={handleRemoveTask}
                                            />
                                        ))}
                                    </div>
                                ))}
                            </div>

                            {tasks.length === 0 && (
                                <div style={{
                                    textAlign: 'center',
                                    padding: '2rem',
                                    color: '#6f6f6f',
                                    border: '1px solid #e0e0e0',
                                    borderRadius: '4px'
                                }}>
                                    Нет добавленных заданий. Нажмите "+" рядом с компетенцией, чтобы добавить задания
                                </div>
                            )}

                            {tasks.length > 0 && (
                                <div style={{
                                    marginTop: '0.5rem',
                                    fontSize: '0.75rem',
                                    color: '#6f6f6f'
                                }}>
                                    Всего: {tasks.length} заданий (минимум 5, максимум 60)
                                </div>
                            )}
                        </FormGroup>
                    )}
                </Form>
            </Modal>

            <SelectCompetenceModelsModal
                isOpen={isSelectModelOpen}
                onClose={() => setIsSelectModelOpen(false)}
                onAdd={handleSelectModel}
                existingCompetenceModelIds={selectedModel ? [selectedModel.modelId] : []}
            />

            <SelectTasksModal
                isOpen={isSelectTasksOpen}
                onClose={() => setIsSelectTasksOpen(false)}
                onAdd={handleAddTasks}
                existingTaskIds={existingTaskIds}
                templateType={Number(type)}
                competenceId={currentCompetenceId}
            />
        </>
    );
};