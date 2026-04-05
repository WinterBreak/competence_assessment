import React from 'react';
import './App.css';
import './components/Navbar';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import {StartAssessment} from "./pages/assessment/start/StartAssessment";
import {Layout} from "./components/Layout";
import {Users} from "./pages/admin/users/Users";
import {Competencies} from "./pages/admin/competencies/Competencies";
import {Tasks} from "./pages/admin/tasks/Tasks";
import {Templates} from "./pages/admin/templates/Templates";
import {CompetenceModels} from "./pages/admin/competendeModels/CompetenceModels";

function App() {
  return (
      <BrowserRouter>
          <Routes>
              <Route path="/" element={<Layout />}>
                  <Route index element={<StartAssessment />} />
                  <Route path="/templates" element={<Templates />} />
                  <Route path="/competence_models" element={<CompetenceModels />} />
                  <Route path="/competencies" element={<Competencies />} />
                  <Route path="/tasks" element={<Tasks />} />
                  <Route path="/users" element={<Users />} />
                  {/*<Route path="users/:id" element={<UserDetail />} />*/}
                  {/* 404 страница */}
                  <Route path="*" element={<Navigate to="/" replace />} />
              </Route>
          </Routes>
      </BrowserRouter>
  );
}

export default App;
