import { Message } from './message';
import { ParserConfiguration } from './parserconfiguration';

export interface MessageSample{
    sampleMessage: Message;
    parseSuccessful: boolean;
    parser: number; // TODO PRJ: How to keep enumeration in sync?
    parserConfiguration: ParserConfiguration;
}