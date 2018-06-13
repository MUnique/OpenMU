interface MyEventTarget extends EventTarget {
    value: string;
}

interface MyFormEvent<T> extends React.FormEvent<T> {
    target: MyEventTarget;
    preventDefault(): void;
}

interface InputProps extends React.HTMLProps<HTMLInputElement> {
    onChange?: React.EventHandler<InputFormEvent>;
}

interface InputFormEvent extends MyFormEvent<HTMLInputElement> {
}