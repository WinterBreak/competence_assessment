import React from 'react';
import './App.css';
import './components/Navbar';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Assessments } from "./pages/assessment/StartAssessment";
import {Layout} from "./components/Layout";
import {Users} from "./pages/admin/users/Users";
import {Competencies} from "./pages/admin/competencies/Competencies";
import {Tasks} from "./pages/admin/tasks/Tasks";
import {Templates} from "./pages/admin/templates/Templates";
import {CompetenceModels} from "./pages/admin/competendeModels/CompetenceModels";
import {AssessmentFormPage} from "./pages/assessment/AssessmentForm";
import {Analytics} from "./pages/assessment/Analytics";
import {CompetenceDashboard} from "./pages/assessment/components/analytics/CompetenceDashboard";

function App() {
  return (
      <BrowserRouter>
          <Routes>
              <Route path="/" element={<Layout />}>
                  <Route index element={<Assessments />} />
                  <Route path="/assessment-form/:id" element={<AssessmentFormPage />} />
                  <Route path="/analytics" element={<CompetenceDashboard />} />
                  <Route path="/results/:id" element={<Analytics />} />
                  <Route path="admin/templates" element={<Templates />} />
                  <Route path="admin/competence_models" element={<CompetenceModels />} />
                  <Route path="admin/competencies" element={<Competencies />} />
                  <Route path="admin/tasks" element={<Tasks />} />
                  <Route path="admin/users" element={<Users />} />
                  {/*<Route path="users/:id" element={<UserDetail />} />*/}
                  {/* 404 страница */}
                  <Route path="*" element={<Navigate to="/" replace />} />
              </Route>
          </Routes>
      </BrowserRouter>
  );
}

export default App;
