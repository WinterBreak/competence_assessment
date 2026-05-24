import React, { useEffect, useState } from 'react';
import { AssessmentResults } from './components/analytics/AssessmentResults';
import { assessmentService } from './services/assessmentService';
import {useParams} from "react-router-dom";
import {Assessment, AssessmentCalcDto, AssessmentCalculation} from "./types/assessment.types";
import {Loading} from "@carbon/react";
import {assessmentStore} from "./stores/assessmentStore";

export const Analytics: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [results, setResults] = useState<AssessmentCalculation>();
    const [assessment, setAssessment] = useState<Assessment | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        if (id) {
            loadAssessment();
            let dto : AssessmentCalcDto = {
                id: id
            }; 
            assessmentService.calculate(dto).then(data => {
                setResults(data);
                setLoading(false);
            });
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

    if (loading) return <Loading description="Загрузка оценки..." withOverlay />;;

    // Маппинг ID компетенций в названия
    
    return (
        <AssessmentResults
            calculation={results!}
            assessment={assessment!}
        />
    );
};