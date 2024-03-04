import { FC, ReactNode, createContext, useContext, useState } from "react";
import Api, { Session } from "../../api/Api";

const SessionContext = createContext<Session>({} as Session);

export const SessionProvider: FC<{ children: ReactNode }> = ({ children }) => {
    const [session] = useState<Session>(Api.create());
    return (
        <SessionContext.Provider value={session}>{children}</SessionContext.Provider>
    )
}

export const useSession = () => {
    const context = useContext(SessionContext);
    if (!context) throw Error('useSession can only be used insde a SessionProvider');
    return context;
}