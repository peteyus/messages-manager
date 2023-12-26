import { Person } from "./person";

export interface Reaction {
  id: number;
  reactionText: string;
  person: Person;
}
