SOLUTION := BmsTelemetry.slnx
PROJECT := BmsTelemetry
TEST_PROJECT := BmsTelemetry.Tests

TAILWIND_INPUT := ./wwwroot/css/tailwind.css
TAILWIND_OUTPUT := ./wwwroot/css/site.css
TAILWIND_CONFIG := ./tailwind.config.js

.PHONY: restore build test run clean tailwind tailwind-build publish

restore:
	dotnet restore $(SOLUTION)

build: restore
	dotnet build $(SOLUTION) --no-restore

test: build
	dotnet test $(TEST_PROJECT) --no-build --verbosity normal

run:
	dotnet run --project $(PROJECT)

clean:
	dotnet clean $(SOLUTION)
	rm -rf $(PROJECT)/bin $(PROJECT)/obj
	rm -rf $(TEST_PROJECT)/bin $(TEST_PROJECT)/obj

tailwind:
	cd $(PROJECT) && npx tailwindcss -c $(TAILWIND_CONFIG) -i $(TAILWIND_INPUT) -o $(TAILWIND_OUTPUT) --watch

tailwind-build:
	cd $(PROJECT) && npx tailwindcss -c $(TAILWIND_CONFIG) -i $(TAILWIND_INPUT) -o $(TAILWIND_OUTPUT) --minify

publish: tailwind-build
	dotnet publish $(PROJECT) \
		-c Release \
		-r win-x64 \
		--self-contained true \
		-o ./publish/win-x64 \
		/p:PublishSingleFile=true \
		/p:EnableCompressionInSingleFile=true
