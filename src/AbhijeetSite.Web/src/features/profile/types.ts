export interface Profile {
  name: string;
  headline: string;
  summary: string;
  about: readonly string[];
  expertise: readonly string[];
  experiences: readonly Experience[];
  educations: readonly Education[];
}

export interface Experience {
  company: string;
  location: string;
  roles: readonly Role[];
}

export interface Role {
  title: string;
  startDate: string;
  endDate: string;
  summary: string;
  achievements: readonly string[];
  focusAreas: readonly string[];
}

export interface Education {
  institution: string;
  credential: string;
  years: string;
  activities: readonly string[];
}
