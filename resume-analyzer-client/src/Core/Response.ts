export type Response = {
    overall_score: number;
    short_summary: string;
    suggestions: Suggestion[];
}

export type Suggestion = {
    category: string, 
    score: number, 
    good: string, 
    bad: string, 
    tip: string
}