import '@mantine/core/styles.css';
import '@mantine/notifications/styles.css';
import './App.css';

import { MantineProvider } from '@mantine/core';
import { Notifications } from '@mantine/notifications';
import { RouterProvider, createBrowserRouter } from 'react-router-dom';
import Layout from './Layout';
import Home from './pages/home/Home';
import Login from './pages/login/Login';
import Manage from './pages/manage/Manage';
import Register from './pages/register/Register';
import Authentication from './routers/Authentication';

function App() {

    const router = createBrowserRouter([
        {
            element: <Layout />,
            children: [
                {
                    path: "/",
                    element: <Home />
                },
                {
                    path: "/login",
                    element: <Login />
                },
                {
                    path: "/register",
                    element: <Register />
                },
                {
                    path: "/manage",
                    element: <Authentication><Manage /></Authentication>
                }
            ]
        }
    ], { basename: import.meta.env.BASE_URL });

    return (
            <MantineProvider>
                <Notifications />
                <RouterProvider router={router} />
            </MantineProvider>
    );
}

export default App;