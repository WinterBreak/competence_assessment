import React, {useEffect, useMemo, useState} from 'react';
import { useNavigate } from 'react-router-dom';
import {
    DataTable,
    Table,
    TableHead,
    TableRow,
    TableHeader,
    TableBody,
    TableCell,
    TableSelectAll,
    TableSelectRow,
    Button,
    Pagination,
    TableContainer,
    TableToolbar,
    TableToolbarContent,
    TableToolbarMenu,
    TableToolbarAction,
    TableToolbarSearch,
    Loading,
    ToastNotification
} from '@carbon/react';
import { observer } from 'mobx-react-lite';
import { TrashCan, Add } from '@carbon/react/icons';
import { assessmentStore } from './stores/assessmentStore';
import { AssessmentCreateModal } from './components/AssessmentCreateModal';
import {Assessment, CreateAssessmentDto} from "./types/assessment.types";

export const Assessments: React.FC = observer(() => {
    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
    const [editingAssessment, setEditingAssessment] = useState<Assessment | null>(null);
    const navigate = useNavigate();
    
    useEffect(() => {
        assessmentStore.loadAssessments();
    }, []);

    const handleAdd = () => {
        setEditingAssessment(null);
        setIsCreateModalOpen(true); // Изменено с setIsModalOpen на setIsCreateModalOpen
    };

    const handleCreateAssessment = async (data: {
        templateId: string;
        type: string;
        candidateId: string;
        inspectorsIds?: string[]
    }) => {
        let newAssessment: CreateAssessmentDto = {
            templateId: Number(data.templateId),
            type: Number(data.type),
            candidateId: Number(data.candidateId),
            inspectorsIds: data.inspectorsIds != undefined
            ? data.inspectorsIds.map(id => Number(id))
            : []    
        }
        await assessmentStore.createAssessment(newAssessment);
        setIsCreateModalOpen(false);
    };

    const handleCreateModalClose = () => {
        setIsCreateModalOpen(false);
    };

    const handleExport = async () => {
        await assessmentStore.exportAssessments();
    };
    
    const assessmentType = (type: string) => {
        switch (Number(type)){
            case 1: return 'Тестирование';
            case 2: return 'Анкетирование';
            case 3: return ' Оценка 360 градусов';
        }
    }

    const rows = useMemo(() =>
            assessmentStore.assessments.map(a => ({
                id: String(a.id),
                type: assessmentType(a.type),
                startDate: a.startDate ? new Date(a.startDate).toLocaleString('ru-RU') : '',
                endDate: a.endDate ? new Date(a.endDate).toLocaleString('ru-RU') : '',
                isFinished: a.isFinished ? 'Завершено' : 'В процессе',
                candidate: a.candidate.fullName,
            })),
        [assessmentStore.assessments]);

    if (assessmentStore.isLoading && assessmentStore.assessments.length === 0) {
        return <Loading description="Загрузка..." withOverlay />;
    }

    const headers = [
        { key: 'type', header: 'Метод оценки' },
        { key: 'startDate', header: 'Дата начала' },
        { key: 'endDate', header: 'Дата завершения' },
        { key: 'isFinished', header: 'Статус' },
        { key: 'candidate', header: 'Аттестуемый' },
    ];

    return (
        <div>
            {assessmentStore.error && (
                <ToastNotification
                    kind="error"
                    title="Ошибка"
                    subtitle={assessmentStore.error}
                    onClose={() => assessmentStore.clearError()}
                />
            )}

            <DataTable rows={rows} headers={headers}>
                {({
                      rows,
                      headers,
                      selectedRows,
                      getTableProps,
                      getHeaderProps,
                      getRowProps,
                      getSelectionProps,
                      getTableContainerProps,
                      getToolbarProps,
                      onInputChange,
                  }) => {

                    const selectedIds = selectedRows.map(r => r.id);

                    return (
                        <TableContainer
                            title="Оценка компетенций"
                            {...getTableContainerProps()}
                        >
                            <TableToolbar {...getToolbarProps()}>
                                <TableToolbarContent>
                                    <TableToolbarSearch
                                        onChange={(e) => {
                                            onInputChange(e);
                                            assessmentStore.setSearchTerm(e.toString());
                                        }}
                                    />

                                    <TableToolbarMenu>
                                        <TableToolbarAction onClick={handleExport}>
                                            Экспорт
                                        </TableToolbarAction>
                                    </TableToolbarMenu>

                                    {selectedIds.length > 0 && (
                                        <Button
                                            kind="danger"
                                            renderIcon={TrashCan}
                                            onClick={() =>
                                                assessmentStore.deleteManyAssessments(selectedIds)
                                            }
                                        >
                                            Удалить ({selectedIds.length})
                                        </Button>
                                    )}

                                    <Button
                                        kind="primary"
                                        onClick={handleAdd}
                                        renderIcon={Add}
                                    >
                                        Добавить
                                    </Button>
                                </TableToolbarContent>
                            </TableToolbar>

                            <Table {...getTableProps()}>
                                <TableHead>
                                    <TableRow>
                                        <TableSelectAll {...getSelectionProps()} />
                                        {headers.map(header => (
                                            <TableHeader {...getHeaderProps({ header })} key={header.key}>
                                                {header.header}
                                            </TableHeader>
                                        ))}
                                    </TableRow>
                                </TableHead>

                                <TableBody>
                                    {rows.map(row => (
                                        <TableRow {...getRowProps({ row })} key={row.id}>
                                            <TableSelectRow {...getSelectionProps({ row })} />

                                            {row.cells.map(cell => {
                                                if (cell.info.header === 'isFinished') {
                                                    const isFinished = cell.value === 'Завершено';
                                                    const assessmentId = row.id;

                                                    return (
                                                        <TableCell key={cell.id}>
                                                            {!isFinished && (
                                                                <span
                                                                    onClick={() => navigate(`/assessment-form/${assessmentId}`)}
                                                                    style={{
                                                                        cursor: 'pointer',
                                                                        color: '#0f62ac',
                                                                        textDecoration: 'underline',
                                                                        fontWeight: 500
                                                                    }}
                                                                >
                        {cell.value}
                    </span>
                                                            )}
                                                            {isFinished && cell.value}
                                                        </TableCell>
                                                    );
                                                }

                                                return (
                                                    <TableCell key={cell.id}>
                                                        {cell.value}
                                                    </TableCell>
                                                );
                                            })}
                                        </TableRow>
                                    ))}
                                </TableBody>
                            </Table>
                        </TableContainer>
                    );
                }}
            </DataTable>

            <Pagination
                page={assessmentStore.currentPage}
                pageSize={assessmentStore.pageSize}
                totalItems={assessmentStore.totalItems}
                pageSizes={[5, 10, 20, 50]}
                onChange={({ page, pageSize }) => {
                    if (pageSize !== assessmentStore.pageSize) {
                        assessmentStore.setPageSize(pageSize);
                    } else {
                        assessmentStore.setCurrentPage(page);
                    }
                }}
            />

            <AssessmentCreateModal
                isOpen={isCreateModalOpen}
                onClose={handleCreateModalClose}
                onSubmit={handleCreateAssessment}
            />
        </div>
    );
});