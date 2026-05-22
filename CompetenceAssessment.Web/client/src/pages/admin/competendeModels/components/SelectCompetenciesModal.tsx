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
    TableToolbar,
    TableToolbarContent,
    TableToolbarSearch,
    Button,
    Stack
} from '@carbon/react';
import { competenceStore } from '../../competencies/stores/competenceStore';
import { observer } from 'mobx-react-lite';
import { Competence } from '../../competencies/types/competence.types';

interface SelectCompetenciesModalProps {
    isOpen: boolean;
    onClose: () => void;
    onAdd: (competencies: Competence[]) => void;
    existingCompetenceIds: string[];
}

export const SelectCompetenciesModal: React.FC<SelectCompetenciesModalProps> = observer(({
                                                                                             isOpen,
                                                                                             onClose,
                                                                                             onAdd,
                                                                                             existingCompetenceIds
                                                                                         }) => {
    const [selectedIds, setSelectedIds] = useState<string[]>([]);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        if (isOpen) {
            competenceStore.loadCompetencies();
            setSelectedIds([]);
            setSearchTerm('');
        }
    }, [isOpen]);
    
    const availableCompetencies = useMemo(() => {
        return competenceStore.competencies.filter(
            comp => !existingCompetenceIds.includes(comp.id)
        );
    }, [competenceStore.competencies, existingCompetenceIds]);
    
    const filteredCompetencies = useMemo(() => {
        if (!searchTerm) return availableCompetencies;
        const term = searchTerm.toLowerCase();
        return availableCompetencies.filter(
            comp => comp.name.toLowerCase().includes(term) ||
                comp.description?.toLowerCase().includes(term)
        );
    }, [availableCompetencies, searchTerm]);

    const handleSelectAll = () => {
        if (selectedIds.length === filteredCompetencies.length) {
            setSelectedIds([]);
        } else {
            setSelectedIds(filteredCompetencies.map(c => c.id));
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
        const selectedCompetencies = competenceStore.competencies.filter(
            c => selectedIds.includes(c.id)
        );
        onAdd(selectedCompetencies);
        onClose();
    };

    const headers = [
        { key: 'name', header: 'Название' },
        { key: 'description', header: 'Описание' }
    ];

    const rows = filteredCompetencies.map(comp => ({
        id: comp.id,
        name: comp.name,
        description: comp.description || ''
    }));

    return (
        <Modal
            open={isOpen}
            modalHeading="Выбор компетенций"
            primaryButtonText="Добавить"
            secondaryButtonText="Отмена"
            onRequestClose={onClose}
            onRequestSubmit={handleAdd}
            size="lg"
            primaryButtonDisabled={selectedIds.length === 0}
        >
            <div style={{ marginBottom: '1rem' }}>
                <TableToolbarSearch
                    persistent
                    placeholder="Поиск компетенций..."
                    onChange={(e) => setSearchTerm(e.toString)}
                />
            </div>

            <TableContainer>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableSelectAll
                                id="competencies"
                                name="competencies"
                                checked={selectedIds.length === filteredCompetencies.length && filteredCompetencies.length > 0}
                                indeterminate={selectedIds.length > 0 && selectedIds.length < filteredCompetencies.length}
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
                            <TableRow key={row.id}>
                                <TableSelectRow
                                    id={row.id}
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

            {filteredCompetencies.length === 0 && (
                <div style={{ textAlign: 'center', padding: '2rem', color: '#6f6f6f' }}>
                    {searchTerm ? 'Ничего не найдено' : 'Нет доступных компетенций'}
                </div>
            )}
        </Modal>
    );
});