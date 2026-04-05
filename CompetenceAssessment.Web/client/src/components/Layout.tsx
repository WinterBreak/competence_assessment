// src/components/Layout/Layout.tsx
import React, {useState} from 'react';
import { Outlet, Link } from 'react-router-dom';
import Navbar from "./Navbar";
import {Content} from "@carbon/react";

export const Layout: React.FC = () => {
    return (
        <div >
            <header>
                <Navbar/>
            </header>
            <Content>
                <Outlet />
            </Content>
            <footer>
            </footer>
        </div>
    );
};