export type Response = {
    overallScore: number;
    shortSummary: string;
    suggestions: Suggestion[];
}

export type Suggestion = {
    category: string, 
    score: number, 
    good: string, 
    bad: string, 
    tip: string
}