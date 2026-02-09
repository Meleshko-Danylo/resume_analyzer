import React, {useEffect, useState} from 'react';
import {useAnalyzerContext} from "./AnalyzerContextProvider";

const AnalyzerResults = () => {
    const {loading, analysisResults, timer, setTimer, lastStartedAt} = useAnalyzerContext();
    
    useEffect(() => {
        let interval: any;
        setTimer(0);
        if(loading){
            const start = Date.now();
            interval = setInterval(() => {
                setTimer(Math.floor((Date.now() - start)/1000));
            }, 1000)
        }
        
        return () => {
            clearInterval(interval);
        }
    }, [loading, lastStartedAt])
    
    const formatTime = (seconds: number) => {
        let minutes = Math.floor(seconds / 60);
        let hours = Math.floor(minutes / 60);
        let days = Math.floor(hours / 24);
        
        let sec = seconds % 60;
        let mins = minutes % 60;
        let hr = hours % 24;
        
        const pad = (num: number):string|number => {
            return num < 10 ? `0${num}` : num;
        }
        
        if(minutes <= 0) return `${pad(sec)}`;
        else if (hours <= 0) return `${pad(mins)}:${pad(sec)}`;
        else if (days <= 0) return `${pad(hr)}:${pad(mins)}:${pad(sec)}`;
        else return `${pad(days)}:${pad(hr)}:${pad(mins)}:${pad(sec)}`;
    }
    
    const getColorForCircle = () => {
        if(timer >= 120) return "bg-orange-400"
        else if (timer >= 180) return "bg-red-400"
        else return "bg-green-400"
    }
    
    if(loading){
        return (
            <div className="analyzer-results-container flex flex-col p-6">
                <div className="pt-1 pr-4 flex justify-end items-center gap-2">
                    <p className="text-[1.2rem] text-[#dbd8e3] opacity-50">
                        {formatTime(timer)}
                    </p>
                    <div className={`w-[10px] h-[10px] rounded-full ${getColorForCircle()}`}></div>
                </div>
                <div className="flex-1 flex items-center justify-center"><img src="/SpinnerSVG.svg" alt="loading" /></div>
            </div>
        );
    }
    
    if(!analysisResults){
        return (
            <div className="analyzer-results-container flex flex-col items-center justify-center p-6 text-[#dbd8e3] opacity-50">
                <p className="text-xl text-center md:text-[22px] lg:text-[26px] xl:text-[22px]">Upload a resume to see the analysis</p>
            </div>
        );
    }
    
    return (
        <div className="analyzer-results-container p-6 text-[#dbd8e3]">
            <div className="flex flex-col sm:flex-row items-center justify-between gap-4 mb-8 bg-[#352f44] p-6 rounded-2xl shadow-lg border border-[#5c5470]">
                <div className="text-center sm:text-left">
                    <h2 className="text-2xl font-bold mb-2">Analysis Results</h2>
                    <p className="text-sm opacity-80">Based on your resume evaluation</p>
                </div>
                <div className="flex flex-col items-center">
                    <div className="relative flex items-center justify-center">
                        <svg className="w-20 h-20 transform -rotate-90">
                            <circle
                                cx="40"
                                cy="40"
                                r="36"
                                stroke="currentColor"
                                strokeWidth="8"
                                fill="transparent"
                                className="text-[#5c5470]"
                            />
                            <circle
                                cx="40"
                                cy="40"
                                r="36"
                                stroke="currentColor"
                                strokeWidth="8"
                                fill="transparent"
                                strokeDasharray={226}
                                strokeDashoffset={226 - (226 * analysisResults.overallScore) / 100}
                                className="text-purple-500 transition-all duration-1000 ease-out"
                            />
                        </svg>
                        <span className="absolute text-xl font-bold">{analysisResults.overallScore}</span>
                    </div>
                    <span className="text-xs mt-2 font-medium uppercase tracking-wider">Overall Score</span>
                </div>
            </div>

            <div className="bg-[#352f44] p-6 rounded-2xl shadow-lg border border-[#5c5470] mb-8">
                <h3 className="text-lg font-semibold mb-3 flex items-center">
                    <span className="mr-2">📝</span> Summary
                </h3>
                <p className="leading-relaxed opacity-90">{analysisResults.shortSummary}</p>
            </div>

            <div className="space-y-6">
                <h3 className="text-lg font-semibold flex items-center px-2">
                    <span className="mr-2">💡</span> Key Suggestions
                </h3>
                {analysisResults.suggestions.map((suggestion, index) => (
                    <div key={index} className="bg-[#352f44] rounded-2xl overflow-hidden border border-[#5c5470] shadow-md">
                        <div className="p-4 border-b border-[#5c5470] flex justify-between items-center bg-[#2a2438]/50">
                            <span className="font-bold text-purple-300">{suggestion.category}</span>
                            <span className="text-sm px-3 py-1 bg-[#5c5470] rounded-full font-mono">{suggestion.score}/100</span>
                        </div>
                        <div className="p-5 space-y-4">
                            <div className="flex gap-3">
                                <span className="text-green-400 mt-1">✓</span>
                                <div>
                                    <p className="text-xs uppercase font-bold text-green-400/70 mb-1">What's Good</p>
                                    <p className="text-sm opacity-90">{suggestion.good}</p>
                                </div>
                            </div>
                            <div className="flex gap-3">
                                <span className="text-red-400 mt-1">✗</span>
                                <div>
                                    <p className="text-xs uppercase font-bold text-red-400/70 mb-1">Needs Work</p>
                                    <p className="text-sm opacity-90">{suggestion.bad}</p>
                                </div>
                            </div>
                            <div className="bg-[#2a2438] p-4 rounded-xl border border-dashed border-[#5c5470]">
                                <p className="text-xs uppercase font-bold text-purple-300 mb-1 italic">Pro Tip</p>
                                <p className="text-sm italic opacity-90">{suggestion.tip}</p>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default AnalyzerResults;