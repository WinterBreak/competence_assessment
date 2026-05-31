import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
    Form,
    TextInput,
    Button,
    Link,
    ToastNotification,
    Loading
} from '@carbon/react';
import {authService} from "./services/authService";
import {LoginRequest} from "./types/auth.types";

interface LoginResponse {
    message: string;
    userId: string;
    email: string;
}

export const Login: React.FC = () => {
    const navigate = useNavigate();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsLoading(true);
        setError(null);

        try {
            let request: LoginRequest = {
                email: email,
                password: password,
            };
            const response = await authService.login(request);
            
            localStorage.setItem('userId', response.userId);
            localStorage.setItem('userEmail', response.email);
            localStorage.setItem('fullName', response.fullName);
            localStorage.setItem('roles', JSON.stringify(response.roles));
            
            navigate('/');
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Произошла ошибка');
        } finally {
            setIsLoading(false);
        }
    };

    const handleForgotPassword = () => {
        console.log('Forgot password clicked');
    };

    return (
        <div style={{
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            minHeight: '100vh',
            backgroundColor: '#f4f4f4'
        }}>
            <div style={{
                width: '100%',
                maxWidth: '400px',
                padding: '2rem',
                backgroundColor: '#ffffff',
                borderRadius: '8px',
                boxShadow: '0 4px 6px rgba(0, 0, 0, 0.1)'
            }}>
                <h1 style={{ marginBottom: '1.5rem', textAlign: 'center' }}>
                    Вход в систему
                </h1>

                {error && (
                    <ToastNotification
                        kind="error"
                        title="Ошибка"
                        subtitle={error}
                        onClose={() => setError(null)}
                        style={{ marginBottom: '1rem' }}
                        lowContrast
                    />
                )}

                <Form onSubmit={handleSubmit}>
                    <TextInput
                        id="email"
                        labelText="Электронная почта"
                        placeholder="example@mail.com"
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                        invalidText="Введите корректный email"
                        disabled={isLoading}
                    />

                    <TextInput
                        id="password"
                        labelText="Пароль"
                        placeholder="Введите пароль"
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        invalidText="Введите пароль"
                        disabled={isLoading}
                        style={{ marginTop: '1rem' }}
                    />

                    <div style={{
                        display: 'flex',
                        justifyContent: 'flex-end',
                        marginTop: '0.5rem',
                        marginBottom: '1.5rem'
                    }}>
                        <Link
                            href="#"
                            onClick={handleForgotPassword}
                            disabled={isLoading}
                        >
                            Забыли пароль?
                        </Link>
                    </div>

                    <Button
                        kind="primary"
                        type="submit"
                        disabled={isLoading}
                        style={{ width: '100%' }}
                    >
                        {isLoading ? 'Вход...' : 'Войти'}
                    </Button>
                </Form>
            </div>

            {isLoading && <Loading description="Авторизация..." withOverlay />}
        </div>
    );
};