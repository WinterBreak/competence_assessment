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
import { TrashCan, Add, Play, View, Incomplete } from '@carbon/react/icons';
import { assessmentStore } from './stores/assessmentStore';
import { AssessmentCreateModal } from './components/AssessmentCreateModal';
import {Assessment, CreateAssessmentDto} from "./types/assessment.types";

export const Assessments: React.FC = observer(() => {
    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
    const [editingAssessment, setEditingAssessment] = useState<Assessment | null>(null);
    const [currentUserId, setCurrentUserId] = useState<string | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        assessmentStore.loadAssessments();

        // Получаем ID текущего пользователя из localStorage
        const userId = localStorage.getItem('userId');
        setCurrentUserId(userId);
    }, []);

    // Проверка, является ли пользователь администратором
    const isAdmin = (): boolean => {
        const roles = localStorage.getItem('roles');
        if (!roles) return false;
        try {
            return JSON.parse(roles).includes('Администратор');
        } catch {
            return false;
        }
    };

    const handleAdd = () => {
        setEditingAssessment(null);
        setIsCreateModalOpen(true);
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

    const assessmentType = (type: string) => {
        switch (Number(type)){
            case 1: return 'Тестирование';
            case 2: return 'Анкетирование';
            case 3: return 'Оценка 360 градусов';
        }
    }

    const assessmentState = (state: number) => {
        switch (state){
            case 1: return 'В процессе';
            case 2: return 'На проверке';
            case 3: return 'Завершено';
            default: return 'Неизвестно';
        }
    }

    // Проверка, является ли текущий пользователь аттестуемым
    const isCurrentUserCandidate = (assessment: Assessment): boolean => {
        if (!assessment){
            return false;
        }
        return currentUserId == assessment.candidate?.id.toString();
    };

    // Проверка, является ли текущий пользователь проверяющим
    const isCurrentUserInspector = (assessment: Assessment): boolean => {
        if (!assessment){
            return false;
        }
        return assessment.inspectors.some(inspector => inspector.id == currentUserId?.toString());
    };

    const rows = useMemo(() =>
            assessmentStore.assessments.map(a => ({
                id: String(a.id),
                type: assessmentType(a.type),
                startDate: a.startDate ? new Date(a.startDate).toLocaleString('ru-RU') : '',
                endDate: a.endDate ? new Date(a.endDate).toLocaleString('ru-RU') : '',
                state: assessmentState(a.state),
                stateCode: a.state,
                candidate: a.candidate.fullName,
                assessment: a
            })),
        [assessmentStore.assessments]);

    if (assessmentStore.isLoading && assessmentStore.assessments.length === 0) {
        return <Loading description="Загрузка..." withOverlay />;
    }

    const headers = [
        { key: 'type', header: 'Метод оценки' },
        { key: 'startDate', header: 'Дата начала' },
        { key: 'endDate', header: 'Дата завершения' },
        { key: 'state', header: 'Статус' },
        { key: 'candidate', header: 'Аттестуемый' },
        { key: 'actions', header: 'Действия' },
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
                                    {rows.map(row => {
                                        const assessment = assessmentStore.assessments.find(a => a.id.toString() === row.id)!;
                                        const state = row.cells.find(cell => cell.info.header === 'state')?.value;

                                        return (
                                            <TableRow {...getRowProps({ row })} key={row.id}>
                                                <TableSelectRow {...getSelectionProps({ row })} />

                                                {row.cells.map(cell => {
                                                    if (cell.info.header === 'state') {
                                                        return (
                                                            <TableCell key={cell.id}>
                                                                {cell.value}
                                                            </TableCell>
                                                        );
                                                    }
                                                    else if (cell.info.header === 'candidate') {
                                                        return (
                                                            <TableCell key={cell.id}>
                                                                {cell.value}
                                                            </TableCell>
                                                        );
                                                    }
                                                    else if (cell.info.header === 'actions') {
                                                        // Для администратора не показываем кнопки
                                                        if (isAdmin()) {
                                                            return <TableCell key={cell.id}>—</TableCell>;
                                                        }

                                                        return (
                                                            <TableCell key={cell.id}>
                                                                <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
                                                                    {/* Кнопка для прохождения оценки (статус "В процессе" и пользователь - аттестуемый) */}
                                                                    {state == 'В процессе' && isCurrentUserCandidate(assessment) && (
                                                                        <Button
                                                                            kind="primary"
                                                                            size="sm"
                                                                            renderIcon={Play}
                                                                            onClick={() => navigate(`/assessment-form/${row.id}`)}
                                                                            style={{ minWidth: 'fit-content' }}
                                                                        >
                                                                            Пройти
                                                                        </Button>
                                                                    )}

                                                                    {/* Кнопка для проверки оценки (статус "На проверке" и пользователь - проверяющий) */}
                                                                    {state == 'На проверке' && isCurrentUserInspector(assessment) && (
                                                                        <Button
                                                                            kind="secondary"
                                                                            size="sm"
                                                                            renderIcon={View}
                                                                            onClick={() => navigate(`/assessment-review/${row.id}`)}
                                                                            style={{ minWidth: 'fit-content' }}
                                                                        >
                                                                            Проверить
                                                                        </Button>
                                                                    )}

                                                                    {/* Кнопка для просмотра результатов (статус "Завершено" и пользователь - аттестуемый) */}
                                                                    {state == 'Завершено' && isCurrentUserCandidate(assessment) && (
                                                                        <Button
                                                                            kind="tertiary"
                                                                            size="sm"
                                                                            renderIcon={Incomplete}
                                                                            onClick={() => navigate(`/results/${row.id}`)}
                                                                            style={{ minWidth: 'fit-content' }}
                                                                        >
                                                                            Результаты
                                                                        </Button>
                                                                    )}
                                                                </div>
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
                                        );
                                    })}
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
                backwardText="Назад"
                forwardText="Вперед"
                itemRangeText={(min, max, total) => `${ min }–${ max } из ${ total } элементов`}
                itemsPerPageText="Элементов на странице"
                pageRangeText={(_current, total) => `из ${ total } ${ total === 1 ? 'страницы' : 'страниц' }`}
                pageSizes={[10, 20, 50]}
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