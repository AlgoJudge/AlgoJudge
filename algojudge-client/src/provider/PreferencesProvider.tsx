import { FC, ReactNode, createContext, useContext, useEffect, useState } from "react";

type ThemeType = "light" | "dark" | undefined;

export interface PreferencesType {
    lang: string | undefined,
    theme: ThemeType,
    setLang: (lang: string) => void,
    setTheme: (theme: ThemeType) => void;
}

const PreferencesContext = createContext<PreferencesType>({} as PreferencesType);

export const PreferencesProvider: FC<{ children: ReactNode }> = ({ children }) => {
    const [lang, setLang2] = useState<string | undefined>(undefined);
    const [theme, setTheme2] = useState<"light" | "dark" | undefined>(undefined);
    const setLang = (lang: string) => {
        setLang2(lang);
    }
    const setTheme = (theme: ThemeType) => {
        console.log("FSDF");
        setTheme2(theme);
    }
    useEffect(() => {
        if (!theme) return;
        localStorage.setItem('theme', theme);
        console.log("SEVE===============", theme);
    }, [theme]);
    useEffect(() => {
        if (!lang) return;
        localStorage.setItem('lang', lang);
    }, [lang]);
    useEffect(() => {
        const lsTheme = localStorage.getItem('theme');
        console.log("REST", lsTheme)
        if (lsTheme) setTheme(lsTheme == 'dark' ? 'dark' : 'light');
        const lsLang = localStorage.getItem('lang');
        if (lsLang) setLang(lsLang);
    }, []);
    return (
        <PreferencesContext.Provider value={{ lang, theme, setLang, setTheme }}>{children}</PreferencesContext.Provider>
    )
}

export const usePreferences = () => {
    const context = useContext(PreferencesContext);
    if (!context) throw Error('usePreferences can only be used insde a PreferencesProvider');
    return context;
}