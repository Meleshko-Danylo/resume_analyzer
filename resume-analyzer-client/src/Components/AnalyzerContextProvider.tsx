import React, {createContext, useContext, useState} from 'react';
import {Response} from "../Core/Response";

const AnalyzerContext = createContext<AnalyzerContextProps|undefined>(undefined);

type AnalyzerContextProps = {
    analysisResults: Response | null;
    setAnalysisResults: React.Dispatch<React.SetStateAction<Response | null>>;
    timer: number;
    setTimer: React.Dispatch<React.SetStateAction<number>>;
    loading: boolean;
    setLoading: React.Dispatch<React.SetStateAction<boolean>>;
    lastStartedAt: number;
    setLastStartedAt: React.Dispatch<React.SetStateAction<number>>;
}

export const useAnalyzerContext = () => {
    const context = useContext(AnalyzerContext);
    if (!context)
        throw new Error("useAnalyzerContext must be used within an Analyzer");
    return context;
}

const AnalyzerContextProvider = ({children}: {children: React.ReactNode}) => {
    const [analysisResults, setAnalysisResults] = useState<Response | null>(null);
    const [loading, setLoading] = useState(false);
    const [timer, setTimer] = useState(0);
    const [lastStartedAt, setLastStartedAt] = useState(0);
    
    return (
        <AnalyzerContext.Provider value={{analysisResults, setAnalysisResults, loading, setLoading, timer, setTimer, lastStartedAt, setLastStartedAt}}>
            {children}
        </AnalyzerContext.Provider>
    );
};

export default AnalyzerContextProvider;