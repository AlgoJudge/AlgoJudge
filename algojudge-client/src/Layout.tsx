import Header from './components/header/Header';
import Footer from './components/footer/Footer';
import { Outlet } from 'react-router-dom';
import { Container } from '@mantine/core';
import { AuthProvider } from './provider/AuthProvider';
import { SessionProvider } from './provider/SessionProvider';

function Layout() {
    return (
        <>
            <SessionProvider>
                <AuthProvider>
                    <Header />
                    <Container size={'lg'} my={40}>
                        <Outlet />
                    </Container>
                    <Footer />
                </AuthProvider>
            </SessionProvider>
        </>
    );
}

export default Layout;