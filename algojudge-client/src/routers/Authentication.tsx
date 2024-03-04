import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../provider/AuthProvider"
import { FC, ReactNode } from "react";

const Authentication: FC<{ children: ReactNode }> = ({ children }) => {
    const { user } = useAuth();
    const location = useLocation();
    const debug = import.meta.env.VITE_APP_DEBUG_AUTHENTICATION == 'true';
    if (!debug && !user?.email) {
        return <Navigate to='/login' state={{ path: location.pathname }} />
    }
    return children;
}

export default Authentication;