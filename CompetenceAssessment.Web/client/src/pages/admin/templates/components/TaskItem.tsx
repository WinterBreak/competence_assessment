import React from 'react';
import { TextInput, Button } from '@carbon/react';
import { TrashCan } from '@carbon/react/icons';

interface TaskItemProps {
    templateId: string;
    competenceId: string;
    taskText: string;
    taskId: string;
    weight: number;
    onWeightChange: (id: string, weight: number) => void;
    onRemove: (id: string) => void;
}

export const TaskItem: React.FC<TaskItemProps> = ({
                                                                  templateId,
                                                                  competenceId,
                                                                  taskText,
                                                                  taskId,
                                                                  weight,
                                                                  onWeightChange,
                                                                  onRemove
                                                              }) => {
    const [weightValue, setWeightValue] = React.useState(String(weight));
    const [error, setError] = React.useState('');

    const handleWeightChange = (value: string) => {
        setWeightValue(value);
        
        const num = parseFloat(value);
        if (isNaN(num)) {
            setError('Введите число');
            return;
        }
        if (num < 0 || num > 1) {
            setError('Значение от 0 до 1');
            return;
        }
        if (value.includes('.') && value.split('.')[1]?.length > 1) {
            setError('Не более одного знака после запятой');
            return;
        }

        setError('');
        onWeightChange(taskId, num)
    };

    return (
        <div style={{
            display: 'flex',
            alignItems: 'flex-start',
            gap: '1rem',
            padding: '1rem',
            borderBottom: '1px solid #e0e0e0',
            backgroundColor: '#ffffff'
        }}>
            <div style={{ flex: 2 }}>
                <div style={{ fontWeight: 'bold', marginBottom: '0.25rem' }}>{taskText}</div>
            </div>

            <div style={{ width: '120px' }}>
                <TextInput
                    id={`weight-${taskId}`}
                    labelText="Вес"
                    value={weightValue}
                    onChange={(e) => handleWeightChange(e.target.value)}
                    invalid={!!error}
                    invalidText={error}
                    size="sm"
                />
            </div>

            <div style={{ paddingTop: '1rem' }}>
                <Button
                    kind="ghost"
                    size="sm"
                    onClick={() => onRemove(taskId)}
                    renderIcon={TrashCan}
                    iconDescription="Удалить"
                    hasIconOnly
                />
            </div>
        </div>
    );
};