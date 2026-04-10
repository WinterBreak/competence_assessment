// src/components/AssessmentCreateModal/SelectTemplateModal.tsx
import React, { useState, useEffect, useMemo } from 'react';
import {
    Modal,
    DataTable,
    Table,
    TableHead,
    TableRow,
    TableHeader,
    TableBody,
    TableCell,
    TableContainer,
    TableToolbar,
    TableToolbarContent,
    TableToolbarSearch,
    Loading,
    MultiSelect,
    Accordion,
    AccordionItem,
} from '@carbon/react';
import { observer } from 'mobx-react-lite';
import { templateStore } from '../../admin/templates/stores/templateStore';
import { Template } from '../../admin/templates/types/template.types';

interface SelectTemplateModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSelect: (template: Template) => void;
    assessmentType: string; // 'testing', 'questionnaire', '360'
}

export const SelectTemplateModal: React.FC<SelectTemplateModalProps> = observer(({
                                                                                     isOpen,
                                                                                     onClose,
                                                                                     onSelect,
                                                                                     assessmentType
                                                                                 }) => {
    const [searchTerm, setSearchTerm] = useState('');
    const [selectedCompetencies, setSelectedCompetencies] = useState<string[]>([]);
    const [expandedRow, setExpandedRow] = useState<string | null>(null);
    const [allCompetencies, setAllCompetencies] = useState<Array<{ id: string; name: string }>>([]);

    useEffect(() => {
        if (isOpen && assessmentType) {
            loadTemplates();
        }
    }, [isOpen, assessmentType]);

    const loadTemplates = async () => {
        await templateStore.loadTemplates();
        
        const competenciesMap = new Map<string, string>();
        templateStore.templates.forEach(template => {
            if (template.competenceModel?.competencies) {
                Object.entries(template.competenceModel.competencies).forEach(([id, name]) => {
                    competenciesMap.set(id, name);
                });
            }
        });
        setAllCompetencies(Array.from(competenciesMap.entries()).map(([id, name]) => ({ id, name })));
    };
    
    const getTemplateTypeFilter = (): string => {
        if (assessmentType == '1') return '1';
        return '2';
    };
    
    const filteredTemplates = useMemo(() => {
        let templates = templateStore.templates.filter(
            t => t.type.toString() === getTemplateTypeFilter()
        );

        // Поиск по названию
        if (searchTerm) {
            templates = templates.filter(t =>
                t.name.toLowerCase().includes(searchTerm.toLowerCase())
            );
        }

        // Фильтрация по компетенциям
        if (selectedCompetencies.length > 0) {
            templates = templates.filter(template => {
                if (!template.competenceModel?.competencies) return false;
                const templateCompetenceIds = Object.keys(template.competenceModel.competencies);
                return selectedCompetencies.some(compId => templateCompetenceIds.includes(compId));
            });
        }

        return templates;
    }, [templateStore.templates, searchTerm, selectedCompetencies, assessmentType]);

    const handleSelectTemplate = (template: Template) => {
        onSelect(template);
        onClose();
        setSearchTerm('');
        setSelectedCompetencies([]);
        setExpandedRow(null);
    };

    const handleClose = () => {
        setSearchTerm('');
        setSelectedCompetencies([]);
        setExpandedRow(null);
        onClose();
    };

    const headers = [
        { key: 'name', header: 'Название шаблона' },
        { key: 'scale', header: 'Шкала' },
        { key: 'competenciesCount', header: 'Компетенций' },
        { key: 'tasksCount', header: 'Заданий' },
    ];

    const rows = filteredTemplates.map(template => ({
        id: template.id.toString(),
        name: template.name,
        scale: `${template.scale}-балльная`,
        competenciesCount: template.competenceModel?.competencies
            ? Object.keys(template.competenceModel.competencies).length
            : 0,
        tasksCount: template.tasks?.length || 0,
    }));

    if (templateStore.isLoading) {
        return (
            <Modal open={isOpen} onRequestClose={handleClose} modalHeading="Выбор шаблона" size="lg">
                <Loading description="Загрузка шаблонов..." />
            </Modal>
        );
    }

    return (
        <Modal
            open={isOpen}
            onRequestClose={handleClose}
            modalHeading="Выбор шаблона"
            size="md"
            primaryButtonText=""
            secondaryButtonText="Отмена"
            primaryButtonDisabled={true}
        >
            <div style={{ marginBottom: '1rem' }}>
                <MultiSelect
                    id="competencies-filter"
                    titleText="Фильтр по компетенциям"
                    items={allCompetencies.map(c => ({ id: c.id, label: c.name }))}
                    itemToString={(item) => item?.label || ''}
                    selectionFeedback="top-after-reopen"
                    onChange={({ selectedItems }) => {
                        selectedItems = selectedItems ?? [];
                        setSelectedCompetencies(selectedItems.map(item => item.id));
                    }}
                    label="Выберите компетенции"
                />
            </div>

            <DataTable rows={rows} headers={headers}>
                {({
                      rows,
                      headers,
                      getTableProps,
                      getHeaderProps,
                      getRowProps,
                      getTableContainerProps,
                      onInputChange,
                  }) => (
                    <TableContainer {...getTableContainerProps()}>
                        <TableToolbar>
                            <TableToolbarContent>
                                <TableToolbarSearch
                                    onChange={(e) => {
                                        onInputChange(e);
                                        setSearchTerm(e.toString());
                                    }}
                                />
                            </TableToolbarContent>
                        </TableToolbar>

                        <Table {...getTableProps()}>
                            <TableHead>
                                <TableRow>
                                    {headers.map(header => (
                                        <TableHeader {...getHeaderProps({ header })} key={header.key}>
                                            {header.header}
                                        </TableHeader>
                                    ))}
                                </TableRow>
                            </TableHead>

                            <TableBody>
                                {rows.map(row => {
                                    const template = row.cells.find(c => c.info.header === 'name')?.value;
                                    const originalTemplate = filteredTemplates.find(t => t.name === template);

                                    return (
                                        <React.Fragment key={row.id}>
                                            <TableRow
                                                {...getRowProps({ row })}
                                                onClick={() => handleSelectTemplate(originalTemplate!)}
                                                style={{ cursor: 'pointer' }}
                                            >
                                                {row.cells.map(cell => (
                                                    <TableCell key={cell.id}>
                                                        {cell.value}
                                                    </TableCell>
                                                ))}
                                            </TableRow>

                                            {expandedRow === row.id && originalTemplate && (
                                                <TableRow>
                                                    <TableCell colSpan={headers.length}>
                                                        <div style={{
                                                            padding: '1rem',
                                                            backgroundColor: '#f4f4f4'
                                                        }}>
                                                            <h4 style={{ marginBottom: '1rem' }}>
                                                                Структура шаблона: {originalTemplate.name}
                                                            </h4>
                                                            {originalTemplate.competenceModel?.competencies && (
                                                                <Accordion>
                                                                    {Object.entries(originalTemplate.competenceModel.competencies).map(([compId, compName]) => {
                                                                        const tasksForCompetence = originalTemplate.tasks?.filter(
                                                                            t => t.competenceId.toString() === compId
                                                                        ) || [];

                                                                        return (
                                                                            <AccordionItem key={compId} title={`${compName} (${tasksForCompetence.length} заданий)`}>
                                                                                {tasksForCompetence.map(task => (
                                                                                    <div key={task.taskId} style={{
                                                                                        padding: '0.75rem',
                                                                                        borderBottom: '1px solid #e0e0e0',
                                                                                        backgroundColor: '#ffffff'
                                                                                    }}>
                                                                                        <div style={{ fontWeight: 'bold', marginBottom: '0.25rem' }}>
                                                                                            {task.taskText}
                                                                                        </div>
                                                                                        <div style={{ fontSize: '0.75rem', color: '#6f6f6f' }}>
                                                                                            Вес: {task.weight}
                                                                                        </div>
                                                                                    </div>
                                                                                ))}
                                                                            </AccordionItem>
                                                                        );
                                                                    })}
                                                                </Accordion>
                                                            )}
                                                        </div>
                                                    </TableCell>
                                                </TableRow>
                                            )}
                                        </React.Fragment>
                                    );
                                })}
                            </TableBody>
                        </Table>
                    </TableContainer>
                )}
            </DataTable>
        </Modal>
    );
});