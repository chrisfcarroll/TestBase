1) The Should.ThrowAsync, Should.NotThrowAsync signatures are messy. All we need are:

Should.ThrowAsync(Func<Task>)
Should.ThrowAsync(Task)
Should.NotThrowAsync(Func<Task>)
Should.NotThrowAsync(Task)

Update the code and tests for just these 4 overloads.

2) Should.ThrowAsync() is risky because if the developer forgets to await it it will always pass. Let's add
Should.Throw(Task)
Should.Throw(Func<Task>)
which will Wait() on the task and then throw or continue.

3) The xml docs for Should.ThrowAsync Should.NotThrowAsync should all use warning emojis in the summary and in a remark, briefly state the problem, and refer to the blocking Should.Throw(Task) as more idiot-proof.