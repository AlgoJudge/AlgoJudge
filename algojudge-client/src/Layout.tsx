import Header from './components/header/Header';
import Footer from './components/footer/Footer';
import { Outlet } from 'react-router-dom';
import { Container } from '@mantine/core';

function Layout() {
    return (
        <>
            <Header />
            <Container size={'lg'} my={40}>
                <Outlet />
            </Container>
            <Footer />
        </>
    );
}

export default Layout;