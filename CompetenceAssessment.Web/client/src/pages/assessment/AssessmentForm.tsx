import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import {Breadcrumb, BreadcrumbItem, Loading, Modal, ToastNotification} from '@carbon/react';
import { assessmentStore } from './stores/assessmentStore';
import { templateStore } from '../admin/templates/stores/templateStore';
import { SurveyForm } from './components/assessmentForm/SurveyForm';
import { TestForm } from './components/assessmentForm/TestForm';
import {Assessment, UpdateAssessmentDto} from './types/assessment.types';

export const AssessmentFormPage: React.FC = observer(() => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [assessment, setAssessment] = useState<Assessment | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [isConfirmationModalOpen, setIsConfirmationModalOpen] = useState(false);
    const [error, setError] = useState('');

    useEffect(() => {
        if (id) {
            loadAssessment();
        }
    }, [id]);

    const loadAssessment = async () => {
        try {
            await assessmentStore.loadAssessmentById(id!);
            const currentAssessment = assessmentStore.currentAssessment;
            if (!currentAssessment) {
                setError('Вам не назначена оценка!');
                return;
            }
            setAssessment(currentAssessment);
        } catch (err) {
            setError('Ошибка при загрузке оценки');
        }
    };

    const handleSubmit = async (answers: Record<string, any>) => {
        setIsSubmitting(true);
        try {
            let dto: UpdateAssessmentDto = {
                assessmentId: id || '',
                answers: answers,
                scores: assessment?.type == '2' || assessment?.type == '3'
                    ? answers
                    : {},
                comments: {},
                comment: '',
            };
            
            await assessmentStore.updateAssessment(dto);
            setIsConfirmationModalOpen(true);
        } catch (err) {
            setError('Ошибка при сохранении результатов');
        } finally {
            setIsSubmitting(false);
        }
    };

    if (!assessment) {
        return <Loading description="Загрузка оценки..." withOverlay />;
    }

    const isTest = assessment.template?.type == '1';
    const template = assessment.template;

    if (!template) {
        return (
            <div style={{ padding: '2rem' }}>
                <ToastNotification
                    kind="error"
                    title="Ошибка"
                    subtitle="Шаблон не найден"
                    onClose={() => setError('')}
                />
            </div>
        );
    }

    return (
        <div style={{ padding: '2rem', backgroundColor: '#f4f4f4', minHeight: '100vh' }}>
            <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
                <Breadcrumb style={{ marginBottom: '2rem' }}>
                    <BreadcrumbItem>
                        <a href="/assessments">Оценки</a>
                    </BreadcrumbItem>
                    <BreadcrumbItem isCurrentPage>
                        Прохождение оценки
                    </BreadcrumbItem>
                </Breadcrumb>

                {error && (
                    <ToastNotification
                        kind="error"
                        title="Ошибка"
                        subtitle={error}
                        onClose={() => setError('')}
                        style={{ marginBottom: '1rem' }}
                    />
                )}

                {isTest ? (
                    <TestForm
                        template={template}
                        onSubmit={handleSubmit}
                        isSubmitting={isSubmitting}
                    />
                ) : (
                    <SurveyForm
                        template={template}
                        onSubmit={handleSubmit}
                        isSubmitting={isSubmitting}
                    />
                )}
            </div>

            <Modal
                open={isConfirmationModalOpen}
                modalHeading="Уведомление"
                primaryButtonText="Ок"
                danger
                onRequestClose={() => setIsConfirmationModalOpen(false)}
                onRequestSubmit={() => navigate('/assessments')}
            >
                <p>Прохождение оценки успешно завершено.</p>
            </Modal>
        </div>
    );
});