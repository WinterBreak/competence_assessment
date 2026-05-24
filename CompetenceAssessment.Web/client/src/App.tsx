import React from 'react';
import './App.css';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Assessments } from "./pages/assessment/StartAssessment";
import { Layout } from "./components/Layout";
import { Users } from "./pages/admin/users/Users";
import { Competencies } from "./pages/admin/competencies/Competencies";
import { Tasks } from "./pages/admin/tasks/Tasks";
import { Templates } from "./pages/admin/templates/Templates";
import { CompetenceModels } from "./pages/admin/competendeModels/CompetenceModels";
import { AssessmentFormPage } from "./pages/assessment/AssessmentForm";
import { Analytics } from "./pages/assessment/Analytics";
import { Login } from "./pages/auth/Login";
import { PrivateRoute } from "./components/PrivateRoute";
import { RoleBasedAnalytics } from "./components/RoleBasedAnalytics";
import {AssessmentHistory} from "./pages/assessment/AssessmentHistory";
import {ReviewForm} from "./pages/assessment/components/assessmentForm/ReviewForm";

function App() {
    return (
        <BrowserRouter>
            <Routes>
                {/* Публичный маршрут - страница входа */}
                <Route path="/login" element={<Login />} />

                {/* Защищенные маршруты */}
                <Route path="/" element={
                    <PrivateRoute>
                        <Layout />
                    </PrivateRoute>
                }>
                    <Route index element={<Assessments />} />
                    <Route path="/assessment-form/:id" element={<AssessmentFormPage />} />
                    <Route path="/assessment-review/:id" element={<ReviewForm />} />
                    {/* Условный маршрут для аналитики */}
                    <Route path="/analytics" element={<RoleBasedAnalytics />} />
                    <Route path="/assessment_history" element={<AssessmentHistory />} />
                    <Route path="/results/:id" element={<Analytics />} />
                    <Route path="admin/templates" element={<Templates />} />
                    <Route path="admin/competence_models" element={<CompetenceModels />} />
                    <Route path="admin/competencies" element={<Competencies />} />
                    <Route path="admin/tasks" element={<Tasks />} />
                    <Route path="admin/users" element={<Users />} />
                    <Route path="*" element={<Navigate to="/" replace />} />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}

export default App;