import './App.css';
import '@mantine/core/styles.css';

import { MantineProvider } from '@mantine/core';
import { RouterProvider, createBrowserRouter } from 'react-router-dom';
import Home from './pages/home/Home';
import Login from './pages/login/Login';
import Register from './pages/register/Register';
import Manage from './pages/manage/Manage';
import Layout from './Layout';

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
                    element: <Manage />
                }
            ]
        }
    ]);

    return (
        <MantineProvider>
            <RouterProvider router={router} />
        </MantineProvider>
    );
}

export default App;