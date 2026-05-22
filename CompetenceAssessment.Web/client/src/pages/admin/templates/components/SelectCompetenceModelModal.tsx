import React, { useState, useEffect, useMemo } from 'react';
import {
    Modal,
    Table,
    TableHead,
    TableRow,
    TableHeader,
    TableBody,
    TableCell,
    TableSelectAll,
    TableSelectRow,
    TableContainer,
    TableToolbarSearch,
} from '@carbon/react';
import { competenceModelStore } from '../../competendeModels/stores/competenceModelStore';
import { observer } from 'mobx-react-lite';
import {CompetenceModel} from "../../competendeModels/types/model.types";

interface SelectCompetenceModelsModalProps {
    isOpen: boolean;
    onClose: () => void;
    onAdd: (competenceModels: CompetenceModel[]) => void;
    existingCompetenceModelIds: string[];
}

export const SelectCompetenceModelsModal: React.FC<SelectCompetenceModelsModalProps> = observer(({
                                                                               isOpen,
                                                                               onClose,
                                                                               onAdd,
                                                                               existingCompetenceModelIds
                                                                           }) => {
    const [selectedIds, setSelectedIds] = useState<string[]>([]);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        if (isOpen) {
            competenceModelStore.loadCompetenceModels();
            setSelectedIds([]);
            setSearchTerm('');
        }
    }, [isOpen]);

    const availableCompetenceModels = useMemo(() => {
        return competenceModelStore.competenceModels.filter(
            comp => !existingCompetenceModelIds.includes(comp.id)
        );
    }, [competenceModelStore.competenceModels, existingCompetenceModelIds]);

    const filteredCompetenceModels = useMemo(() => {
        if (!searchTerm) return availableCompetenceModels;
        const term = searchTerm.toLowerCase();
        return availableCompetenceModels.filter(
            comp => comp.name.toLowerCase().includes(term) ||
                comp.description?.toLowerCase().includes(term)
        );
    }, [availableCompetenceModels, searchTerm]);

    const handleSelectAll = () => {
        if (selectedIds.length === filteredCompetenceModels.length) {
            setSelectedIds([]);
        } else {
            setSelectedIds(filteredCompetenceModels.map(c => c.id));
        }
    };

    const handleSelectRow = (id: string) => {
        if (selectedIds.includes(id)) {
            setSelectedIds(selectedIds.filter(i => i !== id));
        } else {
            setSelectedIds([...selectedIds, id]);
        }
    };

    const handleAdd = () => {
        const selectedModels = competenceModelStore.competenceModels.filter(
            c => selectedIds.includes(c.id)
        );
        onAdd(selectedModels);
        onClose();
    };

    const headers = [
        { key: 'name', header: 'Название' },
        { key: 'description', header: 'Описание' }
    ];

    const rows = filteredCompetenceModels.map(comp => ({
        id: comp.id,
        name: comp.name,
        description: comp.description || ''
    }));

    return (
        <Modal
            open={isOpen}
            modalHeading="Выбор модели компетенций"
            primaryButtonText="Добавить"
            secondaryButtonText="Отмена"
            onRequestClose={onClose}
            onRequestSubmit={handleAdd}
            size="lg"
            primaryButtonDisabled={selectedIds.length === 0}
        >
            <div style={{marginBottom: '1rem'}}>
                <TableToolbarSearch
                    persistent
                    placeholder="Поиск моделей компетенций..."
                    onChange={(e) => setSearchTerm(e.toString)}
                />
            </div>

            <TableContainer>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableSelectAll
                                id="competenceModels"
                                name="competenceModels"
                                checked={selectedIds.length === filteredCompetenceModels.length && filteredCompetenceModels.length > 0}
                                indeterminate={selectedIds.length > 0 && selectedIds.length < filteredCompetenceModels.length}
                                onSelect={handleSelectAll}
                            />
                            {headers.map(header => (
                                <TableHeader key={header.key}>
                                    {header.header}
                                </TableHeader>
                            ))}
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {rows.map(row => (
                            <TableRow key={row.id.toString()}>
                                <TableSelectRow
                                    id={row.id.toString()}
                                    name={row.name}
                                    checked={selectedIds.includes(row.id)}
                                    onSelect={() => handleSelectRow(row.id)}
                                />
                                <TableCell>{row.name}</TableCell>
                                <TableCell>{row.description}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>

            {filteredCompetenceModels.length === 0 && (
                <div style={{textAlign: 'center', padding: '2rem', color: '#6f6f6f'}}>
                    {searchTerm ? 'Ничего не найдено' : 'Нет доступных моделей'}
                </div>
            )}
            <div style={{marginBottom: '1rem', fontSize: '0.875rem', color: '#6f6f6f'}}>
                * Можно выбрать только одну модель
            </div>
        </Modal>
    );
});