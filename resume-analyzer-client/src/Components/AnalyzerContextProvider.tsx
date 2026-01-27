import React, {createContext, useContext, useState} from 'react';
import {Response} from "../Core/Response";

const AnalyzerContext = createContext<AnalyzerContextProps|undefined>(undefined);

type AnalyzerContextProps = {
    analysisResults: Response | null;
    setAnalysisResults: React.Dispatch<React.SetStateAction<Response | null>>;
    loading: boolean;
    setLoading: React.Dispatch<React.SetStateAction<boolean>>;
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
    
    return (
        <AnalyzerContext.Provider value={{analysisResults, setAnalysisResults, loading, setLoading}}>
            {children}
        </AnalyzerContext.Provider>
    );
};

export default AnalyzerContextProvider;